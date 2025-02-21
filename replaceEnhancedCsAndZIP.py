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
        os.makedirs(src_folder)
        os.makedirs(dest_folder)
        return
    
    for file_name in os.listdir(src_folder):
        if file_name.endswith(".cs"):  # Only consider .cs files
            src_file = os.path.join(src_folder, file_name)
            dest_file = find_file_in_directory(file_name, dest_folder)
            
            if dest_file:  # Replace only if file exists in destination
                shutil.copy2(src_file, dest_file)
                #print(f"Replaced: {dest_file} with {src_file}")
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

def move_file(src, dest):
    try:
        os.makedirs(os.path.dirname(dest), exist_ok=True)
        shutil.move(src, dest)
        print(f"File moved successfully to {dest}")
    except Exception as e:
        print(f"Error: {e}")

def move_files(src, dest):
    try:
        os.makedirs(dest, exist_ok=True)
        for file_name in os.listdir(src):
            full_file_name = os.path.join(src, file_name)
            if os.path.isfile(full_file_name):
                shutil.move(full_file_name, os.path.join(dest, file_name))
        print(f"All files moved successfully to {dest}")
    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    class_files_path = "/workspaces/CodeVision1/output/ClassFiles"
    extracted_files_path = "/workspaces/CodeVision1/output/ZIP/Extracted"
    
    # Replace files
    replace_modified_files(class_files_path, extracted_files_path)
    
    #Move merged_output.txt, enhancedClasses to be zipped
    source_file = "/workspaces/CodeVision1/output/merged_output.txt"
    destination_file = "/workspaces/CodeVision1/output/ZIP/Extracted/GenAINumHandler/GenAINumHandler/merged_output.txt" 
    source_dir = "/workspaces/CodeVision1/output/enhancedClassFiles"
    destination_dir = "/workspaces/CodeVision1/output/ZIP/Extracted/GenAINumHandler/GenAINumHandler/DetailedEnhancementInfo"
    move_file(source_file, destination_file)
    move_files(source_dir, destination_dir)

    # Zip the extracted files after replacing 
    zip_file_name = "/workspaces/CodeVision1/output/ZIP/Extracted_files.zip"
    zip_all_files_in_directory(extracted_files_path, zip_file_name)
