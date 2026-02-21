import os
import re
import urllib.parse

def check_links(root_dir):
    markdown_files = []
    for root, dirs, files in os.walk(root_dir):
        for file in files:
            if file.endswith(".md"):
                markdown_files.append(os.path.join(root, file))

    broken_links = []
    
    link_pattern = re.compile(r'\[([^\]]+)\]\(([^)]+)\)')
    
    for file_path in markdown_files:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
            
        matches = link_pattern.findall(content)
        for text, link in matches:
            # Ignore external links
            if link.startswith("http") or link.startswith("mailto:") or link.startswith("#"):
                continue
            
            # Handle anchor links within file (e.g. file.md#anchor)
            target_file = link.split('#')[0]
            if not target_file:
                continue # Anchor only link within same file, skipping for now
                
            # Resolve relative path
            current_dir = os.path.dirname(file_path)
            target_path = os.path.join(current_dir, target_file)
            
            # Normalize path
            target_path = os.path.normpath(target_path)
            
            if not os.path.exists(target_path):
                broken_links.append({
                    "source": file_path,
                    "link": link,
                    "target": target_path
                })
                
    return broken_links

if __name__ == "__main__":
    root_dir = os.path.abspath("docs")
    print(f"Checking links in {root_dir}...")
    broken_links = check_links(root_dir)
    
    if broken_links:
        print(f"Found {len(broken_links)} broken links:")
        for broken in broken_links:
            print(f"  Source: {broken['source']}")
            print(f"  Link:   {broken['link']}")
            print(f"  Target: {broken['target']}")
            print("-" * 20)
    else:
        print("No broken internal links found.")
