import os
import zipfile
import tempfile
import re
import sys

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
                            outfile.write("=" * 80 + "\n\n")
                            print(f"{file} ({file_path})")
                    except Exception as e:
                        print(f"Error reading {file_path}: {e}")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python scanAndMerge.py <directory_to_scan> <output_file>")
        sys.exit(1)

    directory_to_scan = sys.argv[1]
    output_file_path = sys.argv[2]

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
