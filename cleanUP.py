import os
import shutil

# List of files and directories to delete
paths_to_delete = [
    '/workspaces/CodeVision1/output/classFiles/',
    '/workspaces/CodeVision1/output/ZIP/Extracted',
    '/workspaces/CodeVision1/output/merged_output.txt',
    '/workspaces/CodeVision1/input/'
]

# Function to delete a file or directory
def delete_path(path):
    if os.path.exists(path):
        if os.path.isdir(path):
            shutil.rmtree(path)  # Remove directory and its contents
            print(f"Directory {path} deleted successfully.")
        else:
            os.remove(path)  # Remove the file
            print(f"File {path} deleted successfully.")
    else:
        print(f"{path} does not exist.")

# Delete the specified files and directories
for path in paths_to_delete:
    delete_path(path)
