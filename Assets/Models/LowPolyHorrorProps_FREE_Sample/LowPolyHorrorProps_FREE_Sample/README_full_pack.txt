LOW POLY HORROR PROPS  -  v1.0
==============================

45 stylized low poly horror / Halloween props, ready for games.
Graveyard, crypt, witch and haunted-house themes.

CONTENTS
- FBX/       45 models (.fbx)  - Unity, Unreal, Godot, most engines
- GLB/       45 models (.glb)  - Godot, three.js, Babylon, web, Blender
- OBJ/       45 models (.obj + .mtl)
- Textures/  T_HorrorProps_Palette.png   (base color, 256 x 128)
             T_HorrorProps_Emissive.png  (emission: flames, lantern glass, potions)
- LowPolyHorrorProps.blend  - all models, material already set up (Blender 5.2 or newer - use the FBX/GLB files for older versions)
- Previews/  overview sheet and cover images

TECH SPECS
- 45 models, about 13,800 triangles in total (from 20 to ~1,500 per model)
- ONE material and ONE small palette texture for the whole pack
  -> every prop can be batched together, very cheap to render (mobile / VR friendly)
- Flat shaded, real-world scale in meters, pivot at the bottom center of each model
- Clean names: SM_Tombstone_Round, SM_Coffin_Open, SM_Pumpkin_JackOLantern...

SETUP
- Unity: import the FBX folder and the Textures folder. Create one material (URP/Lit or Standard),
  put T_HorrorProps_Palette in Base Map, enable Emission with T_HorrorProps_Emissive (white, intensity 2-4),
  set the texture Filter Mode to "Point" for crisp colors, then assign the material to the models.
- Unreal: import the FBX files with "Import Materials" off, import the two textures (set Filter to "Nearest"),
  make one material: Palette -> Base Color, Emissive x 3 -> Emissive Color, Roughness 0.85.
- Godot: drag the GLB files in: the material and textures are already embedded.
- Blender: open LowPolyHorrorProps.blend.

MODEL LIST
Tombstone_Round, Tombstone_Cross, Tombstone_Broken, Tombstone_Celtic, Obelisk, Grave_WoodenCross,
Coffin_Closed, Coffin_Open, Mausoleum, Fence_Iron, Fence_Pillar, Gate_Iron, DeadTree_Large, DeadTree_Small,
Rock_01, Rock_02, Candle, Candles_Group, Candelabra, Chandelier, Lantern, LampPost, Skull, Bone, Bone_Pile,
Pumpkin, Pumpkin_JackOLantern, Scarecrow, Shovel, Axe, BearTrap, Cage_Hanging, Crate, Barrel, Chair_Old,
Table_Old, Bench, Books_Stack, Cauldron, Potion_Round, Potion_Tall, Potion_Flask, MusicBox, Mirror_Standing,
GrandfatherClock

LICENSE
See LICENSE.txt. In short: use it in as many commercial or free projects as you want, credit appreciated
but not required. Do not resell or redistribute the models themselves.

Thank you for your purchase!
