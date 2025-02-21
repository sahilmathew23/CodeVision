import os
import shutil
import zipfile

def find_file_in_directory(file_name, search_directory):
    """Recursively search for a file in the given directory."""
    for root, _, files in os.walk(search_directory):
        if file_name in files:
            return os.path.join(root, file_name)
    return None

def replace_modified_files(src_folder, dest_folder):
    if not os.path.exists(src_folder) or not os.path.exists(dest_folder):
        print("Source or destination folder does not exist.")
        return
    
    for file_name in os.listdir(src_folder):
        if file_name.endswith(".cs"):  # Only consider .cs files
            src_file = os.path.join(src_folder, file_name)
            dest_file = find_file_in_directory(file_name, dest_folder)
            
            if dest_file:  # Replace only if file exists in destination
                shutil.copy2(src_file, dest_file)
                print(f"Replaced: {dest_file} with {src_file}")
            else:
                print(f"Skipped (not found in destination): {file_name}")

def zip_all_files_in_directory(directory, zip_name):
    """Zip all files under the given directory."""
    with zipfile.ZipFile(zip_name, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(directory):
            for file in files:
                file_path = os.path.join(root, file)
                zipf.write(file_path, os.path.relpath(file_path, directory))
    print(f"Files successfully zipped into {zip_name}")

if __name__ == "__main__":
    class_files_path = "/workspaces/CodeVision1/output/classFiles"
    extracted_files_path = "/workspaces/CodeVision1/output/ZIP/Extracted"
    
    # Replace files
    replace_modified_files(class_files_path, extracted_files_path)
    
    # Zip the extracted files after replacing 
    zip_file_name = "/workspaces/CodeVision1/output/ZIP/Extracted_files.zip"
    zip_all_files_in_directory(extracted_files_path, zip_file_name)
