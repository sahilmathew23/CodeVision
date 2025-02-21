import os
import re

def extract_csharp_content(file_path):
    with open(file_path, "r", encoding="utf-8") as file:
        content = file.read()
    
    # Extract content between ```csharp and ```
    matches = re.findall(r'```csharp(.*?)```', content, re.DOTALL)
    
    # Join matches with newlines to preserve multiple blocks if present
    return '\n'.join(matches).strip()

def process_files(source_directory, target_directory):
    os.makedirs(target_directory, exist_ok=True)
    
    for root, _, files in os.walk(source_directory):
        for file in files:
            if file.endswith(".cs"):
                source_file_path = os.path.join(root, file)
                extracted_content = extract_csharp_content(source_file_path)
                
                # Modify filename by removing 'enhanced_'
                new_file_name = file.replace("enhanced_", "")
                target_file_path = os.path.join(target_directory, new_file_name)
                
                # Save the extracted content to the new location
                with open(target_file_path, "w", encoding="utf-8") as new_file:
                    new_file.write(extracted_content)

directory = "/workspaces/CodeVision1/output/enhancedClassFiles"
target_directory = "/workspaces/CodeVision1/output/ClassFiles"
process_files(directory, target_directory)
print("Processing complete.")
