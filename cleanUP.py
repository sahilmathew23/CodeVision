import os
import shutil

def delete_folder(folder_path):
    if os.path.exists(folder_path):
        try:
            shutil.rmtree(folder_path)
            print(f"Successfully deleted: {folder_path}")
        except Exception as e:
            print(f"Error deleting {folder_path}: {e}")
    else:
        print(f"Folder does not exist: {folder_path}")

def delete_zip_files_in_directory(directory_path):
    if os.path.exists(directory_path):
        for filename in os.listdir(directory_path):
            file_path = os.path.join(directory_path, filename)
            if filename.endswith('.zip') and os.path.isfile(file_path):
                try:
                    os.remove(file_path)
                    print(f"Successfully deleted: {file_path}")
                except Exception as e:
                    print(f"Error deleting {file_path}: {e}")
    else:
        print(f"Directory does not exist: {directory_path}")

if __name__ == "__main__":
    paths_to_delete = [
        '/workspaces/CodeVision1/output/ZIP/Extracted',
        '/workspaces/CodeVision1/output/merged_output.txt',
        '/workspaces/CodeVision1/output/ClassFiles'
    ]
    
    for path in paths_to_delete:
        delete_folder(path)

    # Delete .zip files under /workspaces/CodeVision1/input
    input_folder = '/workspaces/CodeVision1/input'
    delete_zip_files_in_directory(input_folder)
