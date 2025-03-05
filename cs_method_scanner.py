import os
import re

def extract_methods_and_classes(file_content):
    # Pattern to match class definitions
    class_pattern = r'class\s+(\w+)'
    
    # Updated pattern to exclude catch blocks and other keywords
    method_pattern = r'(?<!for\s)(?<!foreach\s)(?<!while\s)(?<!catch\s)(?:public|private|protected|internal|static|\s)+\s+[\w<>[\],\s]+\s+(\w+)\s*\([^)]*\)'
    
    # Keywords to exclude from method names
    excluded_keywords = {'for', 'foreach', 'while', 'catch', 'if', 'switch'}
    
    methods = []
    classes = []
    
    # Find all class definitions
    class_matches = re.finditer(class_pattern, file_content)
    
    for class_match in class_matches:
        class_name = class_match.group(1)
        classes.append(class_name)
        
        # Find the class block using basic bracket matching
        start_pos = class_match.end()
        
        # Extract methods within the class scope
        class_methods = re.finditer(method_pattern, file_content[start_pos:])
        for method_match in class_methods:
            method_name = method_match.group(1)
            # Skip constructors, excluded keywords, and validate it's not a loop condition
            if (method_name != class_name and 
                method_name not in excluded_keywords and 
                not any(method_name.startswith(keyword) for keyword in excluded_keywords)):
                methods.append(f"{class_name}.{method_name}")
    
    return classes, methods

def scan_cs_files(directory):
    all_methods = []
    all_classes = []
    
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                try:
                    with open(file_path, 'r', encoding='utf-8') as f:
                        content = f.read()
                        classes, methods = extract_methods_and_classes(content)
                        all_classes.extend(classes)
                        all_methods.extend(methods)
                except Exception as e:
                    print(f"Error processing {file_path}: {str(e)}")
    
    # Format entries for dropdown
    dropdown_entries = []
    dropdown_entries.extend([f"class:{c}" for c in sorted(set(all_classes))])
    dropdown_entries.extend(sorted(all_methods))
    
    return dropdown_entries

if __name__ == "__main__":
    scan_path = "/workspaces/CodeVision1/output/ZIP/Extracted/"
    entries = scan_cs_files(scan_path)
    for entry in entries:
        print(entry)
