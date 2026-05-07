#!/usr/bin/env python3
import struct
import sys

def check_png_alpha(filepath):
    """Check PNG transparency without PIL"""
    with open(filepath, 'rb') as f:
        # Read PNG header
        header = f.read(8)
        
        # Check if PNG
        if header[:8] != b'\x89PNG\r\n\x1a\n':
            return False
        
        # Read chunks
        while True:
            # Read chunk length
            length_bytes = f.read(4)
            if len(length_bytes) < 4:
                break
            
            length = struct.unpack('>I', length_bytes)[0]
            
            # Read chunk type
            chunk_type = f.read(4)
            
            # Read chunk data
            chunk_data = f.read(length)
            
            # Read CRC
            crc = f.read(4)
            
            # IHDR chunk contains color type
            if chunk_type == b'IHDR':
                width = struct.unpack('>I', chunk_data[0:4])[0]
                height = struct.unpack('>I', chunk_data[4:8])[0]
                bit_depth = chunk_data[8]
                color_type = chunk_data[9]
                
                print(f"Width: {width}, Height: {height}")
                print(f"Bit depth: {bit_depth}")
                print(f"Color type: {color_type}")
                
                # Color types with alpha:
                # 4 = Grayscale with alpha
                # 6 = RGBA (Truecolor with alpha)
                has_alpha = color_type in [4, 6]
                print(f"Has alpha channel: {has_alpha}")
                
                return has_alpha
            
            # Stop after IEND
            if chunk_type == b'IEND':
                break
    
    return False

if __name__ == '__main__':
    result = check_png_alpha('Assets/Sprites/player_character.png')
    print(f"\n[RESULT] PNG transparency: {result}")
