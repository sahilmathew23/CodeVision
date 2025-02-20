import os
import zipfile
import tempfile
import re

auto_generated_regex = re.compile(r"(AssemblyInfo|GlobalUsings\.g|AssemblyAttributes|.*\.g)\.cs$", re.IGNORECASE)

def scan_and_merge_cs_files(directory, output_file):
    with open(output_file, 'w', encoding='utf-8') as outfile:
        for root, _, files in os.walk(directory):
            for file in files:
                if os.path.splitext(file)[1].lower() == ".cs" and not auto_generated_regex.search(file):
                    file_path = os.path.join(root, file)
                    try:
                        with open(file_path, 'r', encoding='utf-8') as infile:
                            content = infile.read()
                            outfile.write(f"===== {file} ({file_path}) =====\n")
                            outfile.write(content + "\n\n")
                            outfile.write("=" * 80 + "\n\n")  # Separator for readability
                            # Print the name and path of each merged file
                            print(f"{file} ({file_path})")
                    except Exception as e:
                        print(f"Error reading {file_path}: {e}")

if __name__ == "__main__": 
    directory_to_scan = "/workspaces/CodeVision1/GenAINumHandler.zip"
    output_file_path = "/workspaces/CodeVision1/merged_output.txt"
    
    if directory_to_scan.endswith(".zip"):
        with tempfile.TemporaryDirectory() as temp_dir:
            try:
                with zipfile.ZipFile(directory_to_scan, 'r') as zip_ref:
                    zip_ref.extractall(temp_dir)
                scan_and_merge_cs_files(temp_dir, output_file_path)
            except Exception as e:
                print(f"Error extracting zip file: {e}")
    else:
        scan_and_merge_cs_files(directory_to_scan, output_file_path)
    
    #print(f"All .cs files have been merged into {output_file_path}")
