import os

def scan_and_merge_cs_files(directory, output_file):
    with open(output_file, 'w', encoding='utf-8') as outfile:
        for root, _, files in os.walk(directory):
            for file in files:
                if file.endswith(".cs") and "Assembly" not in file:  # Ignore files containing 'Assembly'
                    file_path = os.path.join(root, file)
                    try:
                        with open(file_path, 'r', encoding='utf-8') as infile:
                            content = infile.read()
                            outfile.write(f"===== {file} ({file_path}) =====\n")
                            outfile.write(content + "\n\n")
                            outfile.write("=" * 80 + "\n\n")  # Separator for readability
                    except Exception as e:
                        print(f"Error reading {file_path}: {e}")

if __name__ == "__main__": 
    directory_to_scan = "/workspaces/CodeVision1/StudentData"  
    output_file_path = "/workspaces/CodeVision1/merged_output.txt"  
    scan_and_merge_cs_files(directory_to_scan, output_file_path)
    print(f"All .cs files (excluding files containing 'Assembly') have been merged into {output_file_path}")