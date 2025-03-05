import os
import json
import re
import google.generativeai as genai
import networkx as nx
import matplotlib.pyplot as plt
import tiktoken

# Configure Gemini API
genai.configure(api_key="GEMINI_API_KEY")

def calculate_cyclomatic_complexity(code):
    # Count decision points and store metrics
    metrics = {
        "if_statements": len(re.findall(r'\bif\s*\(', code)),
        "else_statements": len(re.findall(r'\belse\s*{', code)),
        "elif_statements": len(re.findall(r'\belse\s+if\s*\(', code)),
        "switch_statements": len(re.findall(r'\bswitch\s*\(', code)),
        "case_branches": len(re.findall(r'\bcase\s+[\w\d_.]+:', code)),
        "while_loops": len(re.findall(r'\bwhile\s*\(', code)),
        "for_loops": len(re.findall(r'\bfor\s*\(', code)),
        "do_while_loops": len(re.findall(r'\bdo\s*{', code)),
        "foreach_loops": len(re.findall(r'\bforeach\s*\(', code)),
        "try_catch_blocks": len(re.findall(r'\bcatch\s*[({\s]', code)),
        "conditional_ops": len(re.findall(r'\?\s*[\w\d_.]+\s*:', code)),  # Ternary operators
        "logical_ops": len(re.findall(r'(?:&&|\|\|)', code))  # Logical AND/OR operators
    }

    # Calculate complexity
    # Base complexity is 1
    # Each decision point adds 1 to complexity
    complexity = 1 + sum(metrics.values())

    return complexity, metrics

# Function to extract classes, methods, function calls, and dependencies
def parse_csharp_code(file_path):
    with open(file_path, "r", encoding="utf-8") as file:
        code = file.read()

    # Existing patterns
    classes = re.findall(r'class\s+(\w+)', code)
    methods = re.findall(r'(public|private|protected)?\s*(\w+)\s+(\w+)\s*\(.*\)', code)
    dependencies = re.findall(r'using\s+([\w\.]+);', code)

    # Calculate complexity for each method
    method_complexities = {}
    method_metrics = {}
    for method in methods:
        method_name = method[2]
        method_pattern = rf'{method_name}\s*\([^)]*\)\s*{{([^{{}}]*(?:{{[^{{}}]*}}[^{{}}]*)*)}}'
        method_matches = re.findall(method_pattern, code, re.DOTALL)
        if method_matches:
            method_code = method_matches[0]
            complexity, metrics = calculate_cyclomatic_complexity(method_code)
            method_complexities[method_name] = complexity
            method_metrics[method_name] = metrics

    # Calculate overall complexity and metrics
    overall_complexity, overall_metrics = calculate_cyclomatic_complexity(code)

    # Extract method calls
    method_calls = []
    method_call_pattern = r'(\w+)\s*\((.*?)\)\s*;?'
    for method_name in methods:
        method_code = re.findall(rf'\b{method_name[2]}\b.*?{{(.*?)}}', code, re.DOTALL)
        if method_code:
            called_methods = re.findall(method_call_pattern, method_code[0])
            for called_method in called_methods:
                method_calls.append((method_name[2], called_method[0]))

    return {
        "file": file_path,
        "classes": classes,
        "methods": [m[2] for m in methods],
        "dependencies": dependencies,
        "method_calls": method_calls,
        "cyclomatic_complexity": {
            "overall": overall_complexity,
            "overall_metrics": overall_metrics,
            "per_method": method_complexities,
            "per_method_metrics": method_metrics
        }
    }

# Step 1: Scan the entire project and store relationships
def scan_project(directory):
    project_data = {}

    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(".cs"):
                file_path = os.path.join(root, file)
                file_data = parse_csharp_code(file_path)
                project_data[file_path] = file_data

    with open("code_index.json", "w", encoding="utf-8") as json_file:
        json.dump(project_data, json_file, indent=4)

    return project_data

# Step 2: Retrieve relevant code (Now includes cross-file context)
def retrieve_related_methods(function_name, visited_methods=None):
    if visited_methods is None:
        visited_methods = set()

    with open("code_index.json", "r", encoding="utf-8") as json_file:
        code_data = json.load(json_file)

    related_methods = set()
    
    # Find methods that directly contain the requested method
    for file_data in code_data.values():
        for method, called_method in file_data["method_calls"]:
            if method == function_name and called_method not in visited_methods:
                visited_methods.add(called_method)
                related_methods.add(called_method)
                # Recursively find methods called by this method
                related_methods.update(retrieve_related_methods(called_method, visited_methods))

    return related_methods

def retrieve_relevant_code(target_name, target_type='method'):
    with open("code_index.json", "r", encoding="utf-8") as json_file:
        code_data = json.load(json_file)

    relevant_files = []
    all_related_items = set()

    for file, file_data in code_data.items():
        if target_type == 'class':
            if target_name in file_data["classes"]:
                relevant_files.append(file)
                all_related_items.add(target_name)
                # Add all methods of this class
                for method in file_data["methods"]:
                    if method.startswith(f"{target_name}."):
                        all_related_items.add(method)
        else:  # method
            for method in file_data["methods"]:
                if method.endswith(f".{target_name}"):
                    relevant_files.append(file)
                    all_related_items.add(target_name)
                    all_related_items.update(retrieve_related_methods(target_name))
                    break

    if not relevant_files:
        return None, []

    combined_code = ""
    for file in relevant_files:
        with open(file, "r", encoding="utf-8") as f:
            combined_code += f"\n\n// File: " + file + "\n" + f.read()

    return combined_code, list(all_related_items)


# Step 3: Retrieve code context using Gemini Flash
def get_code_summary(code_snippet, target_name=None, target_type='method'):
    """
    Enhanced code analysis for refactoring decision support.
    
    Args:
        code_snippet (str): The code to analyze
        target_name (str, optional): The name of the method or class to analyze
        target_type (str, optional): The type of the target ('method' or 'class')
    
    Returns:
        dict: Structured analysis results
    """
    api_key = os.getenv("GEMINI_API_KEY")
    if not api_key:
        print("Error: Gemini API key is not set.")
        return None

    analysis_data = {
        "target_name": target_name,
        "complexity_metrics": {},
        "dependencies": [],
        "code_smells": [],
        "refactoring_suggestions": [],
        "summary": ""
    }

    try:
        # Get method mapping and complexity metrics
        with open("code_index.json", "r", encoding="utf-8") as json_file:
            code_data = json.load(json_file)
            
        if target_name:
            # Analyze callers
            callers = []
            for file_data in code_data.values():
                for caller, called in file_data["method_calls"]:
                    if called == target_name:
                        callers.append(caller)
            analysis_data["dependencies"] = callers

            # Get complexity metrics
            for file_data in code_data.values():
                if target_name in file_data["methods"]:
                    complexity = file_data["cyclomatic_complexity"]
                    metrics = complexity["per_method_metrics"].get(target_name, {})
                    analysis_data["complexity_metrics"] = {
                        "cyclomatic_complexity": complexity["per_method"].get(target_name, 'N/A'),
                        "detailed_metrics": metrics
                    }

        # Prepare enhanced prompt for Gemini
        prompt = f"""Analyze this {target_type} and provide:
1. Brief summary
2. Code smells identified
3. Specific refactoring suggestions
4. Potential risks
5. Estimated refactoring effort (Low/Medium/High)

Code:
{code_snippet}

Current metrics:
{json.dumps(analysis_data["complexity_metrics"], indent=2)}
"""

        encoder = tiktoken.get_encoding("cl100k_base")
        prompt_tokens = len(encoder.encode(prompt))
        print(f"Number of tokens in the prompt: {prompt_tokens}")

        # Get AI analysis
        genai.configure(api_key=api_key)
        model = genai.GenerativeModel(model_name="gemini-2.0-flash")
        response = model.generate_content(prompt)
        
        if response:
            analysis_data["summary"] = response.text
            
            # Extract structured information from response
            # (You might want to add more structure to the prompt to get more structured responses)
            code_smells = re.findall(r"Code smell[s]?:(.*?)(?=\n\n|\Z)", response.text, re.DOTALL)
            refactoring = re.findall(r"Refactoring suggestion[s]?:(.*?)(?=\n\n|\Z)", response.text, re.DOTALL)
            
            analysis_data["code_smells"] = [smell.strip() for smell in code_smells]
            analysis_data["refactoring_suggestions"] = [ref.strip() for ref in refactoring]

        return analysis_data

    except Exception as e:
        print(f"Error in code analysis: {e}")
        return None


# Step 4: Visualize Dependencies
import json
import networkx as nx
import matplotlib.pyplot as plt

def visualize_dependencies():
    with open("code_index.json", "r", encoding="utf-8") as json_file:
        code_data = json.load(json_file)

    G = nx.DiGraph()

    for file_path, file_data in code_data.items():  # Iterate over dictionary items
        for dep in file_data["dependencies"]:
            G.add_edge(file_path, dep)

    plt.figure(figsize=(10, 6))
    nx.draw(G, with_labels=True, node_color='lightblue', edge_color='gray', font_size=8)

    # Save the graph as an image
    plt.savefig("dependencies_graph.png", dpi=300, bbox_inches='tight')
    plt.close()  # Close the plot to prevent overlapping issues in multiple runs



# Example Usage
if __name__ == "__main__":
    import argparse
    parser = argparse.ArgumentParser(description='Code Analysis Tool')
    parser.add_argument('--target', required=True, help='Target name to analyze (class or method name)')
    parser.add_argument('--type', choices=['class', 'method'], default='method', help='Type of target to analyze')
    parser.add_argument('--refact', action='store_true', help='Run in refactoring mode')
    args = parser.parse_args()

    scan_project("/workspaces/CodeVision1/output/ZIP/Extracted/NumHandler")
    code_snippet, related_items = retrieve_relevant_code(args.target, args.type)
    
    if code_snippet is None:
        print(f"No relevant code found for {args.type}: {args.target}")
        exit(1)
        
    print(f"Code Snippet: {code_snippet}")
    print(f"Related Items: {related_items}")
    
    analysis = get_code_summary(code_snippet, args.target, args.type)
    if analysis:
        print(f"Target: {analysis['target_name']}")
        print(f"Complexity: {analysis['complexity_metrics']}")
        print(f"Code Smells: {analysis['code_smells']}")
        print(f"Refactoring Suggestions: {analysis['refactoring_suggestions']}")
        print(f"Detailed Summary: {analysis['summary']}")

    if not args.refact:
        visualize_dependencies()
