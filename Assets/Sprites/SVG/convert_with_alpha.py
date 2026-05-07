#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
SVG to PNG Converter - WITH TRANSPARENCY
Ensures PNG has alpha channel (RGBA, color type 6)
"""

import os
import sys
from pathlib import Path
import struct

# GTK Runtime path
GTK_PATH = "C:/Program Files/GTK3-Runtime Win64/bin"

if not os.path.exists(GTK_PATH):
    print("[ERROR] GTK Runtime not found")
    sys.exit(1)

# Add DLL path
if sys.version_info >= (3, 8):
    os.add_dll_directory(GTK_PATH)
os.environ['PATH'] = GTK_PATH + ';' + os.environ.get('PATH', '')

print("[OK] GTK Runtime ready")

# Import libraries
try:
    from svglib.svglib import svg2rlg
    from reportlab.graphics import renderPM
    from reportlab.lib.utils import ImageReader
    import cairosvg
    print("[OK] Libraries loaded")
except Exception as e:
    print("[ERROR]", str(e))
    sys.exit(1)

# Paths
svg_dir = Path("Assets/Sprites/SVG")
png_dir = Path("Assets/Sprites")

svg_files = [f.name for f in svg_dir.glob("*.svg")]

print(f"\n[CONVERT] Processing {len(svg_files)} SVG files with transparency...\n")

converted = 0
for svg_file in svg_files:
    svg_path = svg_dir / svg_file
    png_file = svg_file.replace(".svg", ".png")
    png_path = png_dir / png_file
    
    print(f"[PROCESS] {svg_file}...")
    
    try:
        # Method 1: Use cairosvg (better transparency support)
        cairosvg.svg2png(
            url=str(svg_path),
            write_to=str(png_path),
            output_width=64,
            output_height=64,
            background_color=None  # Transparent background
        )
        
        # Verify PNG has alpha
        with open(png_path, 'rb') as f:
            f.seek(24)
            color_type = ord(f.read(1))
            has_alpha = color_type in [4, 6]
        
        if has_alpha:
            print(f"[SUCCESS] {svg_file} -> {png_file} (RGBA)")
            converted += 1
        else:
            print(f"[WARN] {png_file} - No alpha, will fix...")
            
            # Fix: Add alpha channel manually using PIL if available
            try:
                from PIL import Image
                img = Image.open(png_path)
                if img.mode != 'RGBA':
                    img = img.convert('RGBA')
                    img.save(png_path, 'PNG')
                    print(f"[FIXED] Added alpha channel")
                    converted += 1
            except ImportError:
                print(f"[WARN] PIL not available, PNG may have background")
                converted += 1
        
    except Exception as e:
        print(f"[ERROR] {svg_file}: {str(e)}")

print(f"\n[COMPLETE] {converted}/{len(svg_files)} files converted")
print("[INFO] PNG files saved to:", png_dir)
print("[NOTE] All PNGs should now have transparent background (RGBA)")
print("\n[NEXT] In Unity Editor:")
print("1. Assets > Refresh")
print("2. Check PNG Inspector - should show transparent background")