import os
import re

def extract_csharp_content(file_path):
    print(f"Reading file: {file_path}")
    with open(file_path, "r", encoding="utf-8") as file:
        content = file.read()
    
    # Extract content between ```csharp and ```
    matches = re.findall(r'```csharp(.*?)```', content, re.DOTALL)
    
    if not matches:
        print(f"Warning: No C# code blocks found in {file_path}, using entire content")
        return content
    
    print(f"Found {len(matches)} C# code blocks in {file_path}")
    return '\n'.join(matches).strip()

def process_files(source_directory, target_directory):
    print(f"\nStarting processing...")
    print(f"Source directory: {source_directory}")
    print(f"Target directory: {target_directory}")
    
    os.makedirs(target_directory, exist_ok=True)
    
    files_processed = 0
    for root, _, files in os.walk(source_directory):
        cs_files = [f for f in files if f.endswith(".cs")]
        print(f"\nFound {len(cs_files)} .cs files in {root}")
        
        for file in cs_files:
            source_file_path = os.path.join(root, file)
            extracted_content = extract_csharp_content(source_file_path)
            
            if not extracted_content:
                print(f"Skipping {file} - no content extracted")
                continue
                
            new_file_name = file.replace("enhanced_", "")
            target_file_path = os.path.join(target_directory, new_file_name)
            print(f"Writing content to: {target_file_path}")
            
            with open(target_file_path, "w", encoding="utf-8") as new_file:
                new_file.write(extracted_content)
            files_processed += 1

    print(f"\nProcessing complete. Total files processed: {files_processed}")

directory = "/workspaces/CodeVision1/output/enhancedFiles"
target_directory = "/workspaces/CodeVision1/output/ClassFiles"
process_files(directory, target_directory)
