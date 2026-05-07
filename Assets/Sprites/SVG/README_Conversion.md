# SVG to PNG Converter - Usage Guide

## 🎯 Purpose

Converts SVG files in `Assets/Sprites/SVG/` to PNG files in `Assets/Sprites/`

## 📂 File Structure

```
Assets/Sprites/
├── player_character.png   ← PNG output (here)
├── player_warrior.png     ← PNG output (here)
├── player_mage.png        ← PNG output (here)
│
└── SVG/
    ├── player_character.svg   ← SVG source
    ├── player_warrior.svg      ← SVG source
    ├── player_mage.svg         ← SVG source
    └── convert_fixed_dll.py    ← This converter script
```

## 🔧 How to Use

### Run Conversion

```bash
cd D:/github/chiisen/survivor.unity
python Assets/Sprites/SVG/convert_fixed_dll.py
```

### What it does:

1. Scans all `.svg` files in `Assets/Sprites/SVG/`
2. Converts each to 64x64 PNG
3. Saves PNG to `Assets/Sprites/` (parent directory)
4. Unity can then use PNGs as sprites

## ✅ Requirements

- **GTK Runtime** installed at: `C:/Program Files/GTK3-Runtime Win64/bin`
- Python packages: `svglib`, `reportlab`

## 📊 Output Settings

- PNG size: **64x64 pixels**
- Recommended Pixels Per Unit: **32**
- Unity size: **2x2 units**

## 🔄 Re-run Conversion

If you modify SVG files, re-run:

```bash
python Assets/Sprites/SVG/convert_fixed_dll.py
```

All PNG files will be regenerated in `Assets/Sprites/`

## 🎨 Available Sprites

### player_character.svg → player_character.png
- Cartoon character with blue body
- Smiling face, red hat
- **Recommended for Survivor game**

### player_warrior.svg → player_warrior.png
- Warrior with silver helmet
- Golden sword, glowing eyes
- Action game style

### player_mage.svg → player_mage.png  
- Mage with purple robe
- Star-decorated hat, magic staff
- Fantasy game style

## 📝 Next Steps After Conversion

1. Unity Editor → Assets/Refresh
2. Check PNG Inspector settings:
   - Texture Type: Sprite (2D and UI)
   - Pixels Per Unit: 32
3. Assign to Player SpriteRenderer