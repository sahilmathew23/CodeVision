import os
import re

def extract_methods(file_content):
    # Pattern to match class definitions
    class_pattern = r'class\s+(\w+)'
    # Pattern to match method definitions while excluding loops
    method_pattern = r'(?<!for\s)(?<!foreach\s)(?<!while\s)(?:public|private|protected|internal|static|\s)+\s+[\w<>[\],\s]+\s+(\w+)\s*\([^)]*\)'
    
    methods = []
    
    # Find all class definitions
    classes = re.finditer(class_pattern, file_content)
    
    for class_match in classes:
        class_name = class_match.group(1)
        # Find the class block using basic bracket matching
        start_pos = class_match.end()
        
        # Extract methods within the class scope
        class_methods = re.finditer(method_pattern, file_content[start_pos:])
        for method_match in class_methods:
            method_name = method_match.group(1)
            # Skip constructors and validate it's not a loop condition
            if method_name != class_name and not any(method_name.startswith(loop) for loop in ['for', 'foreach', 'while']):
                methods.append(f"{class_name}.{method_name}")
    
    return methods

def scan_cs_files(directory):
    all_methods = []
    
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                try:
                    with open(file_path, 'r', encoding='utf-8') as f:
                        content = f.read()
                        methods = extract_methods(content)
                        all_methods.extend(methods)
                except Exception as e:
                    print(f"Error processing {file_path}: {str(e)}")
    
    return sorted(all_methods)

if __name__ == "__main__":
    scan_path = "/workspaces/CodeVision1/output/ZIP/Extracted/"
    
    methods = scan_cs_files(scan_path)
    
    for method in methods:
        print(method)
