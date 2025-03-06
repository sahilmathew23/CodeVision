import os
import sys
import tiktoken  # OpenAI's tokenization library
import google.generativeai as genai  # Gemini API
import requests

def read_file(file_path):
    """Read the content of a file."""
    try:
        with open(file_path, "r", encoding="utf-8") as file:
            return file.read().strip()
    except FileNotFoundError:
        print(f"Error: File {file_path} not found.")
        return None
    except Exception as e:
        print(f"Error reading file {file_path}: {e}")
        return None

def call_gemini_api(prompt):
    """Call the Gemini API with the provided prompt and return the response."""
    api_key = os.getenv("GEMINI_API_KEY")
    if not api_key:
        print("Error: Gemini API key is not set.")
        return None
    
    # Token count estimation
    encoder = tiktoken.get_encoding("cl100k_base")
    prompt_tokens = len(encoder.encode(prompt))
    
    genai.configure(api_key=api_key)
    
    try:
        model = genai.GenerativeModel(model_name="gemini-2.0-flash")
        response = model.generate_content(prompt)
        return response.text if response else None
    except Exception as e:
        print(f"Error calling Gemini API: {e}")
        return None

def call_openai_api(prompt):
    """Call the OpenAI API with the provided prompt and return the response."""
    api_key = os.getenv("OPENAI_API_KEY")
    if not api_key:
        print("Error: OpenAI API key is not set.")
        return None

    url = "https://api.openai.com/v1/chat/completions"
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {api_key}"
    }

    # Token count estimation
    encoder = tiktoken.get_encoding("cl100k_base")
    
    data = {
        "model": "gpt-4-turbo",
        "messages": [{"role": "user", "content": prompt}]
    }
    
    try:
        response = requests.post(url, headers=headers, json=data)
        if response.status_code == 200:
            raw_response = response.json()
            return raw_response.get("choices", [{}])[0].get("message", {}).get("content", None)
        else:
            print(f"Error with API request: {response.status_code} {response.text}")
            return None
    except Exception as e:
        print(f"Error calling OpenAI API: {e}")
        return None

def process_query(user_query, model, project_type):
    """Generate a structured prompt and get a response."""
    # Define file paths
    prompt_template_path = "/workspaces/CodeVision1/input/promptForChat.txt"
    
    if project_type == "raw":
        class_content_path = "/workspaces/CodeVision1/output/merged_output.txt"
    elif project_type == "enhanced":
        class_content_path = "/workspaces/CodeVision1/output/enhanced_project.txt"

    # Read prompt template
    prompt_template = read_file(prompt_template_path)
    if not prompt_template:
        return "Error: Prompt template not available."

    # Read class content
    class_content = read_file(class_content_path)
    if not class_content:
        return "Error: Class content not found."

    # Format the final prompt
    final_prompt = prompt_template.replace("{enhanced_merged_output}", class_content).replace("{userQuery}", user_query)

    if model == "gpt-4-turbo":
            response = call_openai_api(final_prompt)
    elif model == "gemini-2.0-flash":
            response = call_gemini_api(final_prompt)    
    # Call Gemini API
    response = call_gemini_api(final_prompt)
    #print(response)
    return response if response else "Error: Unable to get a response from the AI."

if __name__ == "__main__":
    if len(sys.argv) > 1:
        user_query = sys.argv[1]
        model = sys.argv[2]
        project_type = sys.argv[3]
        print(process_query(user_query, model, project_type))
       
    else:
        print("Error: No user query provided.")
