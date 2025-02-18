import os
import requests
import json
import tiktoken  # OpenAI's tokenization library

def call_openai_api(prompt):
    """Call the OpenAI API with the provided prompt and return the response, along with token usage."""
    api_key = os.getenv("OPENAI_API_KEY")
    url = 'https://api.openai.com/v1/chat/completions'
    headers = {
        'Content-Type': 'application/json',
        'Authorization': f'Bearer {api_key}'
    }

    # Tokenize the prompt to count the tokens
    encoder = tiktoken.get_encoding("cl100k_base")  # Using the encoding for GPT-3.5 turbo models
    prompt_tokens = len(encoder.encode(prompt))  # Get token count of the prompt

    print(f"Number of tokens in the prompt: {prompt_tokens}")

    data = {
        'model': 'o1-2024-12-17',
        'messages': [{'role': 'user', 'content': prompt}]
    }
    try:
        response = requests.post(url, headers=headers, data=json.dumps(data))
        if response.status_code == 200:
            raw_response = response.json()
            print("Raw API Response:", json.dumps(raw_response, indent=4))

            # Extract token usage
            input_tokens = raw_response.get("usage", {}).get("prompt_tokens", "N/A")
            output_tokens = raw_response.get("usage", {}).get("completion_tokens", "N/A")

            print(f"Input Tokens: {input_tokens}")
            print(f"Output Tokens: {output_tokens}")
            print(f"Total Tokens: {input_tokens + output_tokens} ")
            print(f"Model : {data['model']} ")
            

            if 'choices' in raw_response and len(raw_response['choices']) > 0:
                return raw_response['choices'][0]['message']['content']
            else:
                print("No 'choices' or 'content' in the response.")
                return None
        else:
            print(f"Error with API request: {response.status_code} {response.text}")
            return None
    except Exception as e:
        print(f"Error calling OpenAI API: {e}")
        return None

def enhance():
    """Enhance the code content from merged_output.txt using OpenAI API."""
    input_file = "/workspaces/CodeVision1/merged_output.txt"
    prompt_file = "/workspaces/CodeVision1/prompt.txt"
    
    if not os.path.exists(input_file):
        print("Error: Input file not found.")
        return
    if not os.path.exists(prompt_file):
        print("Error: Prompt file not found.")
        return
    
    # Read file content
    with open(input_file, "r", encoding="utf-8") as file:
        file_content_str = file.read()
    
    # Read prompt content
    with open(prompt_file, "r", encoding="utf-8") as file:
        prompt_template = file.read()
    
    # Construct the final prompt
    prompt = prompt_template.format(file_content_str=file_content_str)
    
    # Call OpenAI API
    output = call_openai_api(prompt)
    
    if output:
        output_file = "/workspaces/CodeVision1/enhanced_output.txt"
        with open(output_file, "w", encoding="utf-8") as file:
            file.write(output)
        print(f"Enhanced code has been saved to {output_file}")
    else:
        print("Enhancement process failed.")

# Call the function to execute the enhancement
enhance()
