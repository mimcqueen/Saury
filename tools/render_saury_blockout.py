#!/usr/bin/env python3
"""
Offline Blender EEVEE render of the Saury Unity procedural 3D blockout.
Same hierarchy intent as CharacterKit / StageKit / BabyShowcase / minigame arenas.
Caption: not Play Mode (no Unity license on VM) — real 3D mesh render of the blockout.
"""
import math
import os
import sys

import bpy
from mathutils import Vector, Euler

OUT = "/cursor/stores/bc-03d7f53b-aea5-4b4e-bf03-2125610ed3aa/media/unity-slice"
os.makedirs(OUT, exist_ok=True)

bpy.ops.wm.read_factory_settings(use_empty=True)
scene = bpy.context.scene
scene.render.engine = "BLENDER_EEVEE"
if hasattr(scene.eevee, "taa_render_samples"):
    scene.eevee.taa_render_samples = 64
scene.render.resolution_x = 1280
scene.render.resolution_y = 720
scene.render.image_settings.file_format = "PNG"
scene.world = bpy.data.worlds.new("DarkWorld")
scene.world.use_nodes = True
bg = scene.world.node_tree.nodes["Background"]
bg.inputs[0].default_value = (0.012, 0.01, 0.018, 1)
bg.inputs[1].default_value = 0.55


def mat(name, color, rough=0.72, metal=0.05, emit=None, emit_str=0.0):
    m = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    m.use_nodes = True
    p = m.node_tree.nodes.get("Principled BSDF")
    p.inputs["Base Color"].default_value = (*color, 1)
    p.inputs["Roughness"].default_value = rough
    if "Metallic" in p.inputs:
        p.inputs["Metallic"].default_value = metal
    if emit is not None:
        if "Emission Color" in p.inputs:
            p.inputs["Emission Color"].default_value = (*emit, 1)
            p.inputs["Emission Strength"].default_value = emit_str
        elif "Emission" in p.inputs:
            p.inputs["Emission"].default_value = (*emit, 1)
    return m


def mesh(name, kind, loc, scale, material, rot=(0, 0, 0)):
    if kind == "cube":
        bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    elif kind == "sphere":
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.5, segments=24, ring_count=12, location=loc)
    elif kind == "cylinder":
        bpy.ops.mesh.primitive_cylinder_add(radius=0.5, depth=1, location=loc)
    else:
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.5, location=loc)
    obj = bpy.context.active_object
    obj.name = name
    obj.scale = scale
    obj.rotation_euler = Euler(tuple(math.radians(a) for a in rot), "XYZ")
    if obj.data.materials:
        obj.data.materials[0] = material
    else:
        obj.data.materials.append(material)
    return obj


# Materials
M_FLOOR = mat("floor", (0.14, 0.11, 0.10), 0.9)
M_CURTAIN = mat("curtain", (0.42, 0.28, 0.14), 0.85)
M_WOOD = mat("wood", (0.28, 0.18, 0.11), 0.7)
M_PLASTIC = mat("plastic", (0.32, 0.30, 0.28), 0.65)
M_SUIT = mat("suit", (0.45, 0.12, 0.14), 0.6)
M_SHIRT = mat("shirt", (0.78, 0.72, 0.62), 0.7)
M_SKIN = mat("skin", (0.82, 0.68, 0.55), 0.55)
M_BLACK = mat("black", (0.06, 0.05, 0.06), 0.95)
M_METAL = mat("metal", (0.45, 0.46, 0.48), 0.35, 0.65)
M_EYE = mat("eye", (0.92, 0.90, 0.78), 0.3)
M_IRIS = mat("iris", (0.12, 0.1, 0.08), 0.4)
M_LANE = mat("lane", (0.48, 0.28, 0.16), 0.4)
M_GRASS = mat("grass", (0.22, 0.36, 0.16), 0.95)
M_FLAME = mat("flame", (1.0, 0.42, 0.08), 0.4, 0.0, (1.0, 0.5, 0.1), 5.0)
M_NEON = mat("neon", (1.0, 0.35, 0.55), 0.3, 0.0, (1.0, 0.35, 0.55), 4.0)
M_CYAN = mat("cyan", (0.25, 0.85, 0.95), 0.3, 0.0, (0.25, 0.85, 0.95), 3.0)
M_GRILL = mat("grill", (0.12, 0.12, 0.13), 0.4, 0.45)

CREATURE = [
    (0.78, 0.62, 0.48),
    (0.42, 0.62, 0.32),
    (0.85, 0.55, 0.45),
    (0.35, 0.38, 0.45),
    (0.55, 0.72, 0.55),
    (0.72, 0.28, 0.22),
]


def eyes(parent_loc, head_name_prefix, spread=0.12, up=0.08, forward=0.2, size=0.1):
    # parent_loc Blender (x, y_depth, z_up); eyes sit on +Y face of head
    for i, side in enumerate((-1.0, 1.0)):
        wloc = (parent_loc[0] + side * spread, parent_loc[1] + forward, parent_loc[2] + up)
        mesh(f"{head_name_prefix}_EW{i}", "sphere", wloc, (size, size * 0.6, size * 0.7), M_EYE)
        mesh(f"{head_name_prefix}_IR{i}", "sphere",
             (wloc[0], wloc[1] + size * 0.35, wloc[2]),
             (size * 0.45, size * 0.45, size * 0.45), M_IRIS)


def host(at):
    # at is Blender (x, y_depth, z_up)
    x, y, z = at
    mesh("HostTorso", "cube", (x, y, z + 0.55), (0.62, 0.35, 0.7), M_SUIT)
    mesh("HostShirt", "cube", (x, y + 0.16, z + 0.55), (0.45, 0.08, 0.45), M_SHIRT)
    mesh("HostHead", "sphere", (x, y, z + 1.22), (0.42, 0.4, 0.48), M_SKIN)
    eyes((x, y, z + 1.22), "Host", 0.12, 0.08, 0.2, 0.1)
    mesh("HostMouth", "cube", (x, y + 0.2, z + 1.08), (0.2, 0.06, 0.06), mat("mouth", (0.45, 0.12, 0.15)))
    mesh("HostHair", "sphere", (x + 0.05, y - 0.05, z + 1.35), (0.4, 0.36, 0.25), mat("hair", (0.25, 0.18, 0.12)))
    for side in (-1, 1):
        mesh(f"HostArm{side}", "sphere", (x + side * 0.55, y + 0.25, z + 0.45), (0.14, 0.14, 0.32), M_SUIT)
        mesh(f"HostHand{side}", "sphere", (x + side * 0.45, y + 0.55, z + 0.22), (0.14, 0.14, 0.14), M_SKIN)
    mesh("HostTie", "cube", (x, y + 0.2, z + 0.5), (0.08, 0.04, 0.35), mat("tie", (0.15, 0.2, 0.45)))
    mesh("ChairSeat", "cube", (x, y - 0.35, z + 0.35), (0.7, 0.7, 0.08), M_PLASTIC)
    mesh("ChairBack", "cube", (x, y - 0.65, z + 0.75), (0.7, 0.08, 0.7), M_PLASTIC)


def dad(at, creature, name, scale=1.0):
    # at is Blender (x, y_depth, z_up)
    x, y, z = at
    col = CREATURE[creature]
    body = mat(f"{name}_body", col, 0.65)
    accent = mat(f"{name}_acc", tuple(c * 0.75 for c in col), 0.7)
    s = scale
    for side in (-1, 1):
        mesh(f"{name}_leg{side}", "sphere", (x + side * 0.15 * s, y, z + 0.4 * s), (0.22 * s, 0.22 * s, 0.4 * s), accent)
        mesh(f"{name}_arm{side}", "sphere", (x + side * 0.42 * s, y, z + 1.05 * s), (0.16 * s, 0.16 * s, 0.35 * s), body)
    mesh(f"{name}_torso", "sphere", (x, y, z + 1.0 * s), (0.55 * s, 0.35 * s, 0.45 * s), body)
    hx, hy, hz = x, y, z + 1.55 * s
    mesh(f"{name}_head", "sphere", (hx, hy, hz), (0.42 * s, 0.42 * s, 0.45 * s), body)
    eyes((hx, hy, hz), name, 0.15 * s, 0.1 * s, 0.22 * s, 0.1 * s)
    if creature == 1:
        for side in (-1, 1):
            mesh(f"{name}_ear{side}", "sphere", (hx + side * 0.55 * s, hy, hz + 0.15 * s), (0.35 * s, 0.2 * s, 0.55 * s), body)
    if creature == 2:
        for side in (-1, 1):
            mesh(f"{name}_ear{side}", "cube", (hx + side * 0.28 * s, hy, hz + 0.55 * s), (0.18 * s, 0.1 * s, 0.35 * s), body)
    if creature == 5:
        for side in (-1, 1):
            mesh(f"{name}_horn{side}", "cylinder", (hx + side * 0.2 * s, hy, hz + 0.55 * s), (0.08 * s, 0.08 * s, 0.22 * s), mat(f"{name}_horn", (0.9, 0.85, 0.7)))
    mesh(f"{name}_hat", "cylinder", (hx, hy + 0.05 * s, hz + 0.4 * s), (0.5 * s, 0.5 * s, 0.07 * s), mat(f"{name}_hat", (0.5, 0.15, 0.12)))


def spot(name, loc, color, energy=700, aim=(0.0, 2.2, 1.5)):
    bpy.ops.object.light_add(type="SPOT", location=loc)
    L = bpy.context.active_object
    L.name = name
    L.data.energy = energy
    L.data.color = color
    L.data.spot_size = math.radians(55)
    L.data.shadow_soft_size = 0.5
    direction = Vector(aim) - Vector(loc)
    L.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    mesh(f"{name}_can", "cylinder", loc, (0.35, 0.35, 0.25), M_METAL)
    mesh(f"{name}_lens", "sphere", (loc[0], loc[1], loc[2] - 0.2), (0.28, 0.28, 0.28),
         mat(f"{name}_lens", color, 0.3, 0.0, color, 2.5))


def build_all():
    # Hub set (Blender: X right, Y depth, Z up) — map Unity (x,y,z) -> (x, z, y)
    # Unity stage: floor at y~0, curtains at z=5.6, desk at z=2.15, audience at z=-6
    # Blender: floor Z=0, curtains Y=5.6, desk Y=2.15, audience Y=-6
    def U(x, y, z):
        return (x, z, y)

    mesh("StageFloor", "cube", U(0, -0.1, 0.5), (16, 14, 0.2), M_FLOOR)
    for i in range(7):
        mesh(f"Curtain{i}", "cube", U(-6 + i * 2, 2.6, 5.6), (1.85, 0.12, 5.2), M_CURTAIN)
    mesh("WallL", "cube", U(-8.2, 2.2, 0), (0.3, 14, 5), M_BLACK)
    mesh("WallR", "cube", U(8.2, 2.2, 0), (0.3, 14, 5), M_BLACK)
    mesh("Ceiling", "cube", U(0, 5.2, 0.5), (16, 14, 0.15), M_BLACK)
    mesh("DeskBody", "cube", U(0, 0.55, 2.15), (2.6, 1.1, 1.0), M_WOOD)
    mesh("DeskTop", "cube", U(0, 1.08, 2.2), (2.7, 1.2, 0.08), mat("lam", (0.2, 0.14, 0.1), 0.4))
    mesh("Logo", "cube", U(0, 3.6, 5.35), (3.2, 0.15, 0.7), M_NEON)
    mesh("Riser", "cube", U(0, 0.15, -6.2), (12, 2.5, 0.4), M_BLACK)

    spot("Key", U(1.8, 4.6, -1.5), (1, 0.92, 0.75), 2500)
    spot("Fill", U(-2.2, 4.4, -0.5), (0.7, 0.75, 0.95), 1400)
    spot("Rim", U(0, 4.5, 4.0), (1, 0.7, 0.5), 1000)
    # Soft fill so crushed blacks still read forms
    bpy.ops.object.light_add(type="AREA", location=U(0, 3.5, -2.0))
    area = bpy.context.active_object
    area.data.energy = 400
    area.data.size = 8
    area.data.color = (1.0, 0.9, 0.75)
    area.rotation_euler = Euler((math.radians(50), 0, 0), "XYZ")
    bpy.ops.object.light_add(type="SUN", location=U(2, 6, -3))
    sun = bpy.context.active_object
    sun.data.energy = 1.2
    sun.data.color = (1.0, 0.85, 0.7)
    sun.rotation_euler = Euler((math.radians(45), math.radians(-20), 0), "XYZ")

    host(U(0, 0.95, 2.35))

    for i in range(16):
        row, col = divmod(i, 6)
        p = U(-3.8 + col * 1.45, 0.85 + row * 0.65, -5.6 - row * 0.55)
        for e, side in enumerate((-1, 1)):
            w = mesh(f"AudEW{i}_{e}", "sphere", (p[0] + side * 0.14, p[1], p[2]), (0.16, 0.08, 0.1), M_EYE)
            mesh(f"AudIR{i}_{e}", "sphere", (w.location.x, w.location.y + 0.04, w.location.z), (0.07, 0.07, 0.07), M_IRIS)

    for i in range(4):
        sx = -2.6 + i * 1.7
        mesh(f"Stool{i}", "cylinder", U(sx, 0.45, -1.4), (0.55, 0.55, 0.06), M_PLASTIC)
        mesh(f"Pole{i}", "cylinder", U(sx, 0.22, -1.4), (0.08, 0.08, 0.22), M_METAL)
        dad(U(sx, 0.45, -1.4), i % 6, f"HubDad{i}", 0.85)

    # Baby lane at Unity z=6.8 -> Blender y=6.8
    lane_o = U(0, 0, 6.8)
    def L(x, y, z):
        return (lane_o[0] + x, lane_o[1] + z, lane_o[2] + y)

    mesh("Lane", "cube", L(0, 0.05, 0), (1.8, 9, 0.1), M_LANE)
    mesh("GutterL", "cube", L(-1.15, 0.02, 0), (0.35, 9, 0.12), M_PLASTIC)
    mesh("GutterR", "cube", L(1.15, 0.02, 0), (0.35, 9, 0.12), M_PLASTIC)
    mesh("Hood", "cube", L(0, 1.1, -4.2), (2.4, 0.8, 2.0), M_BLACK)
    mesh("Mouth", "cube", L(0, 0.55, -3.7), (1.4, 0.1, 0.9), M_CYAN)
    mesh("Plinth", "cylinder", L(0, 0.25, 2.5), (1.2, 1.2, 0.25), M_WOOD)
    # Baby on cradle at showcase pose
    bp = L(0, 0.85, 2.3)
    baby_col = mat("baby", (0.78, 0.55, 0.72), 0.55)
    baby_acc = mat("babyA", (0.95, 0.4, 0.55), 0.5)
    mesh("BabyHead", "sphere", (bp[0], bp[1], bp[2] + 0.45), (0.7, 0.7, 0.6), baby_col)
    eyes((bp[0], bp[1], bp[2] + 0.45), "Baby", 0.18, 0.12, 0.28, 0.14)
    for side in (-1, 1):
        mesh(f"BabyEar{side}", "sphere", (bp[0] + side * 0.4, bp[1], bp[2] + 0.5), (0.35, 0.35, 0.35), baby_acc)
    mesh("BabyBody", "sphere", (bp[0], bp[1], bp[2] - 0.05), (0.55, 0.45, 0.32), baby_col)
    mesh("BabyHat", "cylinder", (bp[0], bp[1], bp[2] + 0.75), (0.55, 0.55, 0.05), baby_acc)
    mesh("Cradle", "cube", bp, (0.9, 0.7, 0.2), M_PLASTIC)
    for i in range(6):
        cols = [(1, 0.3, 0.5), (1, 0.7, 0.2), (0.3, 0.9, 1), (0.6, 0.3, 1), (0.3, 1, 0.4), (1, 0.4, 0.3)]
        c = cols[i]
        lx = -1.0 + (i % 3) * 1.0
        lz = -2.2 + (i // 3) * 3.5
        p = L(lx, 2.35, lz)
        mesh(f"Chase{i}", "sphere", p, (0.22, 0.22, 0.22), mat(f"ch{i}", c, 0.3, 0.0, c, 6.0))
        bpy.ops.object.light_add(type="POINT", location=p)
        li = bpy.context.active_object
        li.data.energy = 250
        li.data.color = c
    bpy.ops.object.light_add(type="AREA", location=(0, 8.5, 3.5))
    bl = bpy.context.active_object
    bl.data.energy = 350
    bl.data.size = 4
    bl.data.color = (1.0, 0.85, 0.75)
    bl.rotation_euler = Euler((math.radians(70), 0, 0), "XYZ")

    # Pissing arena @ Unity x=22
    def P(x, y, z):
        return (22 + x, z, y)

    bpy.ops.object.light_add(type="SUN", location=P(0, 8, -4))
    psun = bpy.context.active_object
    psun.data.energy = 2.0
    psun.data.color = (1.0, 0.95, 0.8)
    psun.rotation_euler = Euler((math.radians(50), 0, 0), "XYZ")
    mesh("Yard", "cube", P(0, -0.05, 0), (16, 10, 0.1), M_GRASS)
    mesh("Fence", "cube", P(0, 0.7, 4.6), (16, 0.15, 1.4), M_WOOD)
    for i in range(5):
        mesh(f"Can{i}", "cylinder", P(-3 + i * 1.5, 0.35, 2.6), (0.5, 0.5, 0.35), M_METAL)
    mesh("Pot", "cylinder", P(0, 0.25, 3.6), (0.55, 0.55, 0.25), M_WOOD)
    mesh("Plant", "sphere", P(0, 0.85, 3.6), (0.9, 0.9, 1.1), mat("fol", (0.2, 0.55, 0.25)))
    for i in range(4):
        dad(P(-3.2 + i * 2.0, 0, -2.4), i % 6, f"PissDad{i}")

    # BBQ @ Unity x=-22
    def B(x, y, z):
        return (-22 + x, z, y)

    bpy.ops.object.light_add(type="SUN", location=B(0, 8, -4))
    bsun = bpy.context.active_object
    bsun.data.energy = 2.0
    bsun.data.color = (1.0, 0.9, 0.75)
    bsun.rotation_euler = Euler((math.radians(50), 0, 0), "XYZ")
    mesh("Patio", "cube", B(0, -0.05, 0), (14, 14, 0.1), mat("patio", (0.32, 0.26, 0.18), 0.9))
    mesh("House", "cube", B(0, 1.5, 6.2), (10, 0.4, 3), mat("house", (0.45, 0.35, 0.28)))
    mesh("Grill", "cylinder", B(0, 0.45, 0), (1.5, 1.5, 0.4), M_GRILL)
    mesh("Flame", "sphere", B(0, 1.15, 0), (0.7, 0.7, 1.0), M_FLAME)
    bpy.ops.object.light_add(type="POINT", location=B(0, 1.2, 0))
    fl = bpy.context.active_object
    fl.data.energy = 300
    fl.data.color = (1, 0.45, 0.1)
    mesh("Chair", "cube", B(2.8, 0.55, -1.2), (1.1, 0.12, 1.0), M_PLASTIC)
    mesh("Cooler", "cube", B(-3.2, 0.5, -2.2), (1.4, 0.9, 1.0), mat("cool", (0.18, 0.32, 0.65)))
    for i in range(4):
        ang = i * math.tau / 4
        dad(B(math.cos(ang) * 4.2, 0, math.sin(ang) * 4.2), i % 6, f"BBQDad{i}")


def cam(loc, look, lens=28):
    bpy.ops.object.camera_add(location=loc)
    c = bpy.context.active_object
    c.data.lens = lens
    direction = Vector(look) - Vector(loc)
    c.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    return c


def render(c, filename):
    scene.camera = c
    path = os.path.join(OUT, filename)
    scene.render.filepath = path
    bpy.ops.render.render(write_still=True)
    print("WROTE", path, os.path.getsize(path), file=sys.stderr)


def main():
    # Remove old concept stills
    for f in os.listdir(OUT):
        if f.endswith(".png"):
            os.remove(os.path.join(OUT, f))

    build_all()

    # Cameras in Blender space (x, y depth, z up)
    shots = [
        # Blender: +Y is into stage (curtains), Host at y≈2.35, dads at y≈-1.4
        ((4.8, -5.2, 2.6), (0.0, 1.5, 1.4), 28, "01-hub-lobby.png"),
        ((0.9, 0.2, 1.9), (0.0, 2.5, 1.65), 32, "02-host-desk.png"),
        ((-2.8, -4.0, 2.3), (-0.5, -1.2, 1.3), 30, "03-make-dad-hub.png"),
        ((22.0, -9.0, 5.0), (22.0, 1.0, 0.9), 28, "04-pissing-contest.png"),
        # Baby lane sits at Blender y≈6.8; cradle/baby around y≈9.1
        ((3.8, 7.2, 2.6), (0.0, 9.2, 1.1), 28, "05-baby-showcase.png"),
        ((-18.5, -8.0, 4.2), (-22.0, 0.5, 1.0), 28, "06-bbq-brawl.png"),
    ]
    for loc, look, lens, fn in shots:
        render(cam(loc, look, lens), fn)

    with open(os.path.join(OUT, "README.md"), "w") as f:
        f.write(
            """# Unity slice media

**3D blockout stills** — offline Blender EEVEE renders of the same procedural hierarchy Unity builds at runtime (`StageKit` / `CharacterKit` / arenas / baby lane).

| File | Beat |
|------|------|
| `01-hub-lobby.png` | Maury hub set + Host + dad stools |
| `02-host-desk.png` | CPU Host at desk (modeled mesh kit) |
| `03-make-dad-hub.png` | Hub dad creature blockouts |
| `04-pissing-contest.png` | 3D yard / cans / plant arena |
| `05-baby-showcase.png` | Bowling lane + cradle baby + chase lights |
| `06-bbq-brawl.png` | Backyard grill brawl arena |

**Honest caption:** These are **not** Unity Play Mode / Game-view captures. This cloud VM has Unity Editor **6000.3.26f1** installed but **no Unity license login**, so Play Mode is blocked. Stills are real 3D mesh+light renders matching the project’s procedural blockout. On a licensed Hub install: Play Bootstrap → menu **Saury → Capture 3D Slice Screenshots** to replace with live Game-view PNGs.
"""
        )
    print("DONE", file=sys.stderr)


if __name__ == "__main__":
    main()
