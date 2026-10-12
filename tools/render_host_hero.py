#!/usr/bin/env python3
"""Force a readable Host desk hero still — same kits, dead-simple camera + key."""
import math, os, sys
import bpy
from mathutils import Vector, Euler

# Load helpers from blockout (runs factory reset + defs)
src = open("/workspace/tools/render_saury_blockout.py").read()
# Stop before main()
pre, _, _ = src.partition("\ndef main(")
exec(pre, globals())

OUT = "/cursor/stores/bc-03d7f53b-aea5-4b4e-bf03-2125610ed3aa/media/unity-slice"
os.makedirs(OUT, exist_ok=True)

# Fresh scene already from exec; clear leftover default if any
for o in list(bpy.data.objects):
    bpy.data.objects.remove(o, do_unlink=True)

build_hub()

# Debug dump
for name in ("Head", "Torso", "DeskBody", "DeskTop", "MicHead", "NameGlow", "Shirt", "FrontGlow"):
    o = bpy.data.objects.get(name)
    if o:
        print("OBJ", name, "loc", tuple(round(c, 3) for c in o.location), file=sys.stderr)
    else:
        print("MISSING", name, file=sys.stderr)

# Strong readable key on host face (in addition to spots)
bpy.ops.object.light_add(type="AREA", location=(0.8, 1.2, 2.4))
area = bpy.context.active_object
area.data.energy = 160
area.data.size = 2.2
area.data.color = (1.0, 0.9, 0.78)
direction = Vector((0, 2.8, 1.7)) - Vector(area.location)
area.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()

# Front hero — audience looking at host over desk
# Host head ~ (0, 2.79, 1.97), desk top ~ y=2.2 z=1.1
loc = (1.8, -0.2, 1.85)
look = (0.0, 2.75, 1.65)
c = cam(loc, look, lens=35)
scene.camera = c
path = os.path.join(OUT, "02-host-desk.png")
scene.render.filepath = path
bpy.ops.render.render(write_still=True)
print("WROTE", path, os.path.getsize(path), file=sys.stderr)

# Also write a debug alt
loc2 = (0.15, -1.5, 1.55)
look2 = (0.0, 2.8, 1.55)
c2 = cam(loc2, look2, lens=32)
scene.camera = c2
path2 = "/tmp/host-front-debug.png"
scene.render.filepath = path2
bpy.ops.render.render(write_still=True)
print("WROTE", path2, os.path.getsize(path2), file=sys.stderr)
print("DONE", file=sys.stderr)
