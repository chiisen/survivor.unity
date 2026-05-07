#!/usr/bin/env python3
"""
Force convert PNG to RGBA (with transparency)
Uses PIL/Pillow to ensure alpha channel exists
"""

from PIL import Image
import sys

png_files = [
    'Assets/Sprites/player_character.png',
    'Assets/Sprites/player_warrior.png',
    'Assets/Sprites/player_mage.png'
]

print('[FIX] Converting PNGs to RGBA with transparency...\n')

for png_file in png_files:
    try:
        # Load PNG
        img = Image.open(png_file)
        
        print(f'[LOAD] {png_file}')
        print(f'  Mode: {img.mode}')
        
        # Convert to RGBA if needed
        if img.mode != 'RGBA':
            img = img.convert('RGBA')
            
            # Make white background transparent
            datas = img.getdata()
            new_data = []
            for item in datas:
                # If pixel is close to white, make it transparent
                if item[0] > 220 and item[1] > 220 and item[2] > 220:
                    new_data.append((255, 255, 255, 0))  # Transparent
                else:
                    new_data.append(item)
            
            img.putdata(new_data)
            
            # Save
            img.save(png_file, 'PNG')
            print(f'  [FIXED] Converted to RGBA with transparency')
        else:
            print(f'  [OK] Already has alpha channel')
        
    except Exception as e:
        print(f'  [ERROR] {str(e)}')

print('\n[DONE] All PNGs should now have transparency')
print('[NEXT] Unity Editor > Assets > Refresh')