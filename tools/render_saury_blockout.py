#!/usr/bin/env python3
"""
Art pass 2 — Blender EEVEE hero stills matching Unity StageKit/CharacterKit.
Eerie dingy Maury / Liar's Bar: spots crush the world, connected dolls, bowling pomp.
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
    scene.eevee.taa_render_samples = 96
if hasattr(scene.eevee, "use_bloom"):
    scene.eevee.use_bloom = True
    scene.eevee.bloom_intensity = 0.06
    scene.eevee.bloom_threshold = 1.1
scene.render.resolution_x = 1280
scene.render.resolution_y = 720
scene.render.image_settings.file_format = "PNG"
scene.world = bpy.data.worlds.new("Void")
scene.world.use_nodes = True
bg = scene.world.node_tree.nodes["Background"]
bg.inputs[0].default_value = (0.005, 0.004, 0.008, 1)
bg.inputs[1].default_value = 0.08


def mat(name, color, rough=0.65, metal=0.04, emit=None, emit_str=0.0):
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
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.5, segments=32, ring_count=16, location=loc)
    elif kind == "cylinder":
        bpy.ops.mesh.primitive_cylinder_add(radius=0.5, depth=1, location=loc)
    else:
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.5, segments=28, ring_count=14, location=loc)
    obj = bpy.context.active_object
    obj.name = name
    obj.scale = scale
    obj.rotation_euler = Euler(tuple(math.radians(a) for a in rot), "XYZ")
    if obj.data.materials:
        obj.data.materials[0] = material
    else:
        obj.data.materials.append(material)
    bpy.ops.object.shade_smooth()
    return obj


def limb(name, loc, length, radius, material, rot=(0, 0, 0)):
    """Connected joint-bone-joint along -Z local after rotation; authored in Z-up world."""
    root_loc = loc
    # joint A
    mesh(f"{name}_jA", "sphere", root_loc, (radius * 2.1, radius * 2.1, radius * 2.1), material)
    # bone center
    # apply rot roughly as vertical limb downward
    bone = mesh(f"{name}_bone", "sphere", (root_loc[0], root_loc[1], root_loc[2] - length * 0.5),
                (radius * 2, radius * 2, length), material)
    bone.rotation_euler = Euler(tuple(math.radians(a) for a in rot), "XYZ")
    mesh(f"{name}_jB", "sphere", (root_loc[0], root_loc[1], root_loc[2] - length),
         (radius * 2.05, radius * 2.05, radius * 2.05), material)


# Palette
M_BLACK = mat("black", (0.02, 0.018, 0.022), 0.95)
M_FLOOR = mat("floor", (0.16, 0.12, 0.09), 0.9)
M_CURTAIN = mat("curtain", (0.48, 0.32, 0.16), 0.88)
M_CURTAIN2 = mat("curtain2", (0.22, 0.12, 0.07), 0.9)
M_WOOD = mat("wood", (0.32, 0.2, 0.12), 0.7)
M_LAM = mat("lam", (0.18, 0.12, 0.09), 0.4)
M_PLASTIC = mat("plastic", (0.38, 0.34, 0.3), 0.6)
M_SUIT = mat("suit", (0.38, 0.08, 0.1), 0.45)
M_SUIT2 = mat("suit2", (0.22, 0.04, 0.06), 0.5)
M_SHIRT = mat("shirt", (0.82, 0.74, 0.58), 0.55)
M_SKIN = mat("skin", (0.9, 0.74, 0.58), 0.45)
M_HAIR = mat("hair", (0.18, 0.12, 0.08), 0.7)
M_EYE = mat("eye", (0.95, 0.9, 0.75), 0.25)
M_IRIS = mat("iris", (0.12, 0.08, 0.05), 0.4)
M_METAL = mat("metal", (0.42, 0.42, 0.44), 0.3, 0.65)
M_LANE = mat("lane", (0.55, 0.32, 0.16), 0.35)
M_LANE_OIL = mat("laneoil", (0.62, 0.4, 0.2), 0.2)
M_PINK = mat("pink", (1.0, 0.28, 0.5), 0.3, 0.0, (1.0, 0.28, 0.5), 4.5)
M_AMBER = mat("amber", (1.0, 0.7, 0.25), 0.3, 0.0, (1.0, 0.7, 0.25), 3.5)
M_CYAN = mat("cyan", (0.25, 0.9, 0.95), 0.3, 0.0, (0.25, 0.9, 0.95), 3.5)
M_GRASS = mat("grass", (0.28, 0.38, 0.16), 0.95)
M_GRILL = mat("grill", (0.1, 0.1, 0.11), 0.4, 0.45)
M_FLAME = mat("flame", (1.0, 0.4, 0.08), 0.35, 0.0, (1.0, 0.45, 0.1), 6.0)
M_BRUISE = mat("bruise", (0.35, 0.22, 0.32), 0.55)
M_CHAIN = mat("chain", (0.5, 0.52, 0.48), 0.4, 0.55)

CREATURE = [
    ((0.8, 0.62, 0.48), (0.55, 0.18, 0.14)),
    ((0.4, 0.58, 0.28), (0.25, 0.4, 0.18)),
    ((0.88, 0.58, 0.48), (0.95, 0.75, 0.8)),
    ((0.32, 0.36, 0.42), (0.55, 0.5, 0.35)),
    ((0.5, 0.7, 0.52), (0.7, 0.85, 0.7)),
    ((0.7, 0.26, 0.22), (0.95, 0.85, 0.55)),
]


def U(x, y, z):
    """Unity (x,y,z) → Blender (x, z, y) with Z-up."""
    return (x, z, y)


def spot(name, loc, aim, color, energy=1800, size=42):
    bpy.ops.object.light_add(type="SPOT", location=loc)
    L = bpy.context.active_object
    L.name = name
    L.data.energy = energy
    L.data.color = color
    L.data.spot_size = math.radians(size)
    L.data.shadow_soft_size = 0.35
    if hasattr(L.data, "use_custom_distance"):
        L.data.use_custom_distance = True
        L.data.cutoff_distance = 60.0
    direction = Vector(aim) - Vector(loc)
    L.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    # Fixture slightly above light so it never occludes the cone
    mesh(f"{name}_can", "cylinder", (loc[0], loc[1], loc[2] + 0.12), (0.32, 0.32, 0.22), M_METAL)
    mesh(f"{name}_lens", "sphere", (loc[0], loc[1], loc[2] - 0.05), (0.22, 0.22, 0.22),
         mat(f"{name}_em", color, 0.3, 0.0, color, 2.5))


def face(head_loc, prefix, spread=0.11, big=False, toward=-1.0):
    """toward: Blender Y sign for face direction (−1 = audience / −Y, +1 = stage / +Y)."""
    s = 0.24 if big else 0.16
    fy = toward
    for i, side in enumerate((-1, 1)):
        wloc = (head_loc[0] + side * spread, head_loc[1] + fy * 0.22, head_loc[2] + 0.06)
        mesh(f"{prefix}_ew{i}", "sphere", wloc, (s, s * 0.55, s * 0.7), M_EYE)
        mesh(f"{prefix}_ir{i}", "sphere", (wloc[0], wloc[1] + fy * s * 0.35, wloc[2]),
             (s * 0.4, s * 0.4, s * 0.4), M_IRIS)


def host():
    # Sit clearly behind desk (audience at -Y). Unity host ~z=2.4; stills push to 2.85 for readable doll.
    base = U(0, 0.55, 2.85)
    x, y, z = base
    mesh("ChairSeat", "cube", (x, y - 0.15, z + 0.38), (0.75, 0.7, 0.1), M_PLASTIC)
    mesh("ChairBack", "cube", (x, y + 0.15, z + 0.85), (0.75, 0.1, 0.85), M_PLASTIC)
    mesh("Cushion", "cube", (x, y + 0.1, z + 0.95), (0.65, 0.08, 0.5), M_BRUISE)
    mesh("Hip", "sphere", (x, y, z + 0.42), (0.55, 0.42, 0.28), M_SUIT)
    mesh("Torso", "sphere", (x, y - 0.02, z + 0.82), (0.6, 0.36, 0.58), M_SUIT)
    mesh("Belly", "sphere", (x, y - 0.12, z + 0.64), (0.52, 0.36, 0.36), M_SUIT2)
    mesh("Shirt", "cube", (x, y - 0.18, z + 0.82), (0.4, 0.06, 0.44), M_SHIRT)
    mesh("Tie", "cube", (x, y - 0.22, z + 0.74), (0.08, 0.04, 0.4), mat("tie", (0.12, 0.18, 0.4)))
    mesh("TieClip", "cube", (x, y - 0.24, z + 0.64), (0.1, 0.02, 0.04), M_AMBER)
    for side in (-1, 1):
        mesh(f"Shoulder{side}", "sphere", (x + side * 0.4, y, z + 1.08), (0.22, 0.22, 0.22), M_SUIT)
        # Arms reach toward audience / desk top (-Y)
        mesh(f"Upper{side}", "sphere", (x + side * 0.5, y - 0.18, z + 0.85), (0.16, 0.16, 0.4), M_SUIT)
        mesh(f"Joint{side}", "sphere", (x + side * 0.52, y - 0.32, z + 0.68), (0.15, 0.15, 0.15), M_SUIT)
        mesh(f"Fore{side}", "sphere", (x + side * 0.5, y - 0.5, z + 0.52), (0.14, 0.14, 0.34), M_SUIT)
        mesh(f"Hand{side}", "sphere", (x + side * 0.48, y - 0.68, z + 0.38), (0.14, 0.14, 0.14), M_SKIN)
    mesh("Neck", "sphere", (x, y - 0.02, z + 1.15), (0.2, 0.2, 0.18), M_SKIN)
    h = (x, y - 0.06, z + 1.42)
    mesh("Head", "sphere", h, (0.44, 0.42, 0.5), M_SKIN)
    mesh("Hair", "sphere", (h[0] + 0.04, h[1] + 0.02, h[2] + 0.18), (0.4, 0.36, 0.26), M_HAIR)
    mesh("Combover", "sphere", (h[0] + 0.16, h[1] - 0.04, h[2] + 0.04), (0.22, 0.24, 0.14), M_HAIR)
    face(h, "Host", spread=0.13, toward=-1.0)
    mesh("Nose", "sphere", (h[0], h[1] - 0.24, h[2] - 0.02), (0.12, 0.16, 0.13), M_SKIN)
    mesh("Mouth", "cube", (h[0], h[1] - 0.22, h[2] - 0.16), (0.22, 0.08, 0.06), mat("mouth", (0.5, 0.15, 0.18)))
    mesh("Brow", "sphere", (h[0], h[1] - 0.18, h[2] + 0.12), (0.24, 0.08, 0.08),
         mat("brow", (0.95, 0.85, 0.7), 0.35, 0.0, (1, 0.95, 0.85), 0.4))


def dad(at_u, creature, name, scale=0.88):
    x, y, z = U(*at_u)
    body_c, acc_c = CREATURE[creature]
    body = mat(f"{name}_b", body_c, 0.5)
    acc = mat(f"{name}_a", acc_c, 0.45)
    limb_c = mat(f"{name}_l", tuple(c * 0.88 for c in body_c), 0.5)
    s = scale
    mesh(f"{name}_pelvis", "sphere", (x, y, z + 0.72 * s), (0.45 * s, 0.32 * s, 0.28 * s), limb_c)
    for side in (-1, 1):
        mesh(f"{name}_thigh{side}", "sphere", (x + side * 0.14 * s, y, z + 0.45 * s), (0.18 * s, 0.18 * s, 0.45 * s), limb_c)
        mesh(f"{name}_foot{side}", "sphere", (x + side * 0.16 * s, y + 0.06 * s, z + 0.08 * s), (0.18 * s, 0.26 * s, 0.1 * s), acc)
        mesh(f"{name}_arm{side}", "sphere", (x + side * 0.4 * s, y, z + 1.1 * s), (0.15 * s, 0.15 * s, 0.4 * s), limb_c)
        mesh(f"{name}_hand{side}", "sphere", (x + side * 0.55 * s, y + 0.1 * s, z + 0.95 * s), (0.14 * s, 0.14 * s, 0.14 * s), body)
    mesh(f"{name}_torso", "sphere", (x, y, z + 1.1 * s), (0.5 * s, 0.34 * s, 0.5 * s), body)
    mesh(f"{name}_neck", "sphere", (x, y, z + 1.42 * s), (0.16 * s, 0.16 * s, 0.12 * s), body)
    h = (x, y + 0.02 * s, z + 1.62 * s)
    mesh(f"{name}_head", "sphere", h, (0.4 * s, 0.4 * s, 0.44 * s), body)
    # Dads face the host desk (+Y / stage)
    face(h, name, 0.12 * s, big=(creature in (1, 4)), toward=+1.0)
    if creature == 1:
        for side in (-1, 1):
            mesh(f"{name}_ear{side}", "sphere", (h[0] + side * 0.5 * s, h[1], h[2] + 0.1 * s), (0.25 * s, 0.16 * s, 0.5 * s), body)
    if creature == 2:
        for side in (-1, 1):
            mesh(f"{name}_ear{side}", "sphere", (h[0] + side * 0.2 * s, h[1], h[2] + 0.45 * s), (0.14 * s, 0.1 * s, 0.28 * s), body)
    if creature == 5:
        for side in (-1, 1):
            mesh(f"{name}_horn{side}", "cylinder", (h[0] + side * 0.16 * s, h[1], h[2] + 0.45 * s), (0.07 * s, 0.07 * s, 0.2 * s), acc)
    if creature == 0:
        mesh(f"{name}_mullet", "sphere", (h[0], h[1] - 0.28 * s, h[2]), (0.28 * s, 0.22 * s, 0.2 * s), M_HAIR)
    mesh(f"{name}_hat", "cylinder", (h[0], h[1] + 0.04 * s, h[2] + 0.38 * s), (0.48 * s, 0.48 * s, 0.07 * s), acc)


def build_hub():
    mesh("VoidFloor", "cube", U(0, -0.15, 0.2), (28, 24, 0.1), M_BLACK)
    mesh("StageFloor", "cylinder", U(0, -0.02, 0.8), (11, 9, 0.08), M_FLOOR)
    for i in range(9):
        c = M_CURTAIN if i % 2 == 0 else M_CURTAIN2
        mesh(f"Curtain{i}", "cube", U(-7 + i * 1.75, 2.7, 5.5), (1.7, 0.18, 5.4), c)
    mesh("Proscenium", "cube", U(0, 5.0, 5.2), (16, 0.5, 0.6), M_BLACK)
    mesh("WallL", "cube", U(-8.5, 2.4, 0), (0.4, 16, 5.5), M_BLACK)
    mesh("WallR", "cube", U(8.5, 2.4, 0), (0.4, 16, 5.5), M_BLACK)
    mesh("Ceiling", "cube", U(0, 5.4, 0.5), (18, 16, 0.2), M_BLACK)
    mesh("Riser", "cube", U(0, 0.2, -6.4), (14, 2.8, 0.5), M_BLACK)

    # Desk — audience at -Y; nameplate/mic face the room
    mesh("DeskBody", "cube", U(0, 0.55, 2.2), (2.8, 1.15, 1.05), M_WOOD)  # Blender (x, depth, height)
    mesh("DeskTop", "cube", U(0, 1.1, 2.2), (2.95, 1.25, 0.07), M_LAM)
    mesh("FrontGlow", "cube", U(0, 0.55, 1.62), (2.6, 0.05, 0.1), M_PINK)
    mesh("Nameplate", "cube", U(0, 0.72, 1.64), (1.05, 0.06, 0.3), M_PLASTIC)
    mesh("NameGlow", "cube", U(0, 0.72, 1.6), (0.9, 0.03, 0.14), M_AMBER)
    mesh("MicBase", "cylinder", U(0.55, 1.15, 2.0), (0.14, 0.14, 0.04), M_METAL)
    mesh("MicArm", "cylinder", U(0.55, 1.38, 1.85), (0.045, 0.045, 0.4), M_METAL, rot=(50, 0, 0))
    mesh("MicHead", "sphere", U(0.55, 1.62, 1.7), (0.15, 0.15, 0.15), M_METAL)
    mesh("Mug", "cylinder", U(-0.45, 1.18, 1.95), (0.1, 0.1, 0.14), mat("mug", (0.55, 0.45, 0.35)))
    mesh("Ashtray", "cylinder", U(0.15, 1.14, 1.9), (0.12, 0.12, 0.04), M_METAL)
    mesh("CRT", "cube", U(-0.85, 1.35, 2.35), (0.55, 0.35, 0.4), M_PLASTIC)
    mesh("CRTScreen", "cube", U(-0.85, 1.35, 2.15), (0.42, 0.05, 0.28), mat("crt", (0.1, 0.2, 0.15), 0.3, 0.0, (0.3, 0.9, 0.45), 2.5))

    # Logo
    mesh("LogoFrame", "cube", U(0, 3.75, 5.25), (3.6, 0.2, 0.95), M_BLACK)
    mesh("Logo", "cube", U(0, 3.75, 5.18), (3.2, 0.12, 0.7), M_PINK)
    for i in range(5):
        mesh(f"Letter{i}", "cube", U(-1.2 + i * 0.6, 3.75, 5.1), (0.35, 0.05, 0.4), M_AMBER)

    # Buckshot ref 05 — hanging practical cans, crushed blacks
    spot("Key", U(1.4, 4.4, 0.2), U(0, 1.55, 2.7), (1, 0.88, 0.62), 2200, 28)
    spot("Key2", U(-1.3, 4.35, 0.4), U(0, 1.45, 2.5), (1, 0.78, 0.5), 1600, 26)
    spot("Face", (0.9, 1.1, 2.5), (0.0, 2.75, 1.85), (1, 0.96, 0.88), 1200, 18)
    spot("Dad", U(-1.8, 4.2, -2.2), U(0, 1.0, -1.3), (1, 0.82, 0.58), 900, 34)
    spot("Rim", U(0, 4.5, 4.4), U(0, 1.6, 2.6), (1, 0.45, 0.32), 500, 24)
    # Soft face fill so the doll reads without washing the void
    bpy.ops.object.light_add(type="AREA", location=(0.6, 0.9, 2.2))
    fill = bpy.context.active_object
    fill.name = "HostFill"
    fill.data.energy = 150
    fill.data.size = 2.0
    fill.data.color = (1.0, 0.9, 0.78)
    fill.rotation_euler = (Vector((0.0, 2.8, 1.7)) - Vector(fill.location)).to_track_quat("-Z", "Y").to_euler()

    host()

    # Audience eyes
    for i in range(28):
        row, col = divmod(i, 7)
        p = U(-4.5 + col * 1.35, 0.75 + row * 0.55, -5.5 - row * 0.5)
        for e, side in enumerate((-1, 1)):
            w = mesh(f"Aud{i}_{e}", "sphere", (p[0] + side * 0.13, p[1], p[2]), (0.15, 0.08, 0.1), M_EYE)
            mesh(f"AudI{i}_{e}", "sphere", (w.location.x, w.location.y + 0.035, w.location.z), (0.06, 0.06, 0.06), M_IRIS)
            mesh(f"AudC{i}_{e}", "sphere", (w.location.x + 0.02, w.location.y + 0.045, w.location.z + 0.02),
                 (0.025, 0.025, 0.025), mat(f"catch{i}{e}", (1, 0.95, 0.7), 0.2, 0.0, (1, 0.95, 0.7), 2.5))

    for i in range(4):
        sx = -2.7 + i * 1.8
        mesh(f"Stool{i}", "cylinder", U(sx, 0.48, -1.5), (0.55, 0.55, 0.06), M_PLASTIC)
        mesh(f"Pole{i}", "cylinder", U(sx, 0.25, -1.5), (0.07, 0.07, 0.22), M_METAL)
        dad((sx, 0.48, -1.5), i % 6, f"HubDad{i}", 0.88)


def build_baby_lane():
    lo = U(0, 0, 7.0)

    def L(x, y, z):
        return (lo[0] + x, lo[1] + z, lo[2] + y)

    mesh("Lane", "cube", L(0, 0.06, 0.2), (1.7, 9.5, 0.1), M_LANE)
    mesh("LaneOil", "cube", L(0, 0.12, 0.2), (1.5, 8.5, 0.02), M_LANE_OIL)
    mesh("GutterL", "cube", L(-1.1, 0.0, 0.2), (0.4, 9.5, 0.18), M_PLASTIC)
    mesh("GutterR", "cube", L(1.1, 0.0, 0.2), (0.4, 9.5, 0.18), M_PLASTIC)
    mesh("Hood", "cube", L(0, 1.2, -4.4), (2.6, 1.0, 2.2), M_BLACK)
    mesh("Mouth", "cube", L(0, 0.7, -3.75), (1.35, 0.08, 0.95), M_CYAN)
    mesh("Plinth", "cylinder", L(0, 0.28, 2.8), (1.35, 1.35, 0.28), M_WOOD)
    mesh("PlinthRing", "cylinder", L(0, 0.56, 2.8), (1.2, 1.2, 0.03), M_AMBER)

    # Cradle + baby at showcase pose
    bp = L(0, 0.95, 2.6)
    mesh("Cradle", "cube", bp, (0.95, 0.75, 0.18), M_PLASTIC)
    mesh("Pillow", "sphere", (bp[0], bp[1], bp[2] + 0.12), (0.7, 0.5, 0.15), M_BRUISE)
    baby = mat("baby", (0.82, 0.55, 0.72), 0.45)
    acc = mat("babyA", (0.95, 0.35, 0.55), 0.4)
    h = (bp[0], bp[1], bp[2] + 0.55)
    mesh("BabyHead", "sphere", h, (0.72, 0.72, 0.65), baby)
    face(h, "Baby", 0.16, big=True, toward=-1.0)
    for side in (-1, 1):
        mesh(f"BabyEar{side}", "sphere", (h[0] + side * 0.45, h[1], h[2] + 0.05), (0.32, 0.32, 0.32), acc)
        mesh(f"BabyArm{side}", "sphere", (bp[0] + side * 0.38, bp[1], bp[2] + 0.2), (0.12, 0.12, 0.25), baby)
    mesh("BabyBody", "sphere", (bp[0], bp[1], bp[2] + 0.05), (0.5, 0.4, 0.35), baby)
    mesh("BabyHat", "cylinder", (h[0], h[1], h[2] + 0.42), (0.55, 0.55, 0.05), acc)

    for i in range(8):
        cols = [(1, 0.3, 0.55), (1, 0.7, 0.25), (0.3, 0.95, 1), (0.7, 0.3, 1),
                (0.3, 1, 0.45), (1, 0.45, 0.3), (1, 0.9, 0.3), (0.5, 0.4, 1)]
        c = cols[i]
        lx = -1.15 + (i % 4) * 0.75
        lz = -2.4 + (i // 4) * 4.2
        p = L(lx, 2.45, lz)
        mesh(f"Chase{i}", "sphere", p, (0.2, 0.2, 0.2), mat(f"ch{i}", c, 0.25, 0.0, c, 7.0))
        bpy.ops.object.light_add(type="POINT", location=p)
        li = bpy.context.active_object
        li.data.energy = 350
        li.data.color = c
        if hasattr(li.data, "use_custom_distance"):
            li.data.use_custom_distance = True
            li.data.cutoff_distance = 40.0

    spot("BabyHero", L(1.2, 2.8, 1.5), L(0, 1.4, 2.7), (1, 0.92, 0.78), 3200, 30)
    spot("BabyFill", L(-1.0, 2.4, 3.2), L(0, 1.2, 2.7), (1, 0.85, 0.7), 1400, 36)


def build_piss():
    """Grimy PS1 bathroom (ref 06) — tiles, sinks, stalls."""
    def P(x, y, z):
        return (22 + x, z, y)

    M_TILE = mat("tile", (0.42, 0.4, 0.36), 0.85)
    M_TILE_D = mat("tiled", (0.35, 0.3, 0.24), 0.88)
    M_PORC = mat("porc", (0.72, 0.7, 0.65), 0.4)
    mesh("TileFloor", "cube", P(0, -0.05, 0), (14, 11, 0.1), M_TILE)
    for i in range(5):
        mesh(f"Void{i}", "cube", P(-4 + i * 2.1, -0.02, -1.5 + (i % 2) * 2.2), (0.7, 0.7, 0.08), M_BLACK)
    mesh("WallBack", "cube", P(0, 1.8, 5.2), (14, 0.25, 3.6), M_TILE_D)
    mesh("WallL", "cube", P(-7, 1.8, 0), (0.25, 11, 3.6), M_TILE)
    mesh("WallR", "cube", P(7, 1.8, 0), (0.25, 11, 3.6), M_TILE)
    for i in range(3):
        z = -1.5 + i * 1.6
        mesh(f"Basin{i}", "cube", P(-6.2, 0.95, z), (1.1, 0.7, 0.18), M_PORC)
        mesh(f"Mirror{i}", "cube", P(-6.2, 1.85, z - 0.35), (1.0, 0.04, 0.85), mat(f"mir{i}", (0.25, 0.28, 0.3), 0.25))
    mesh("Stall", "cube", P(5.2, 1.4, 0.5), (0.12, 4.5, 2.8), mat("stall", (0.22, 0.2, 0.18)))
    mesh("StallDoor", "cube", P(4.4, 1.3, -1.8), (1.2, 0.08, 2.4), mat("sdoor", (0.28, 0.26, 0.24)))
    for i in range(5):
        mesh(f"Can{i}", "cylinder", P(-3 + i * 1.5, 0.35, 2.4), (0.5, 0.5, 0.35), M_METAL)
    mesh("Pot", "cylinder", P(0, 0.25, 3.5), (0.55, 0.55, 0.25), M_WOOD)
    mesh("Plant", "sphere", P(0, 0.85, 3.5), (0.9, 0.9, 1.1), mat("fol", (0.22, 0.48, 0.22)))
    spot("BathSpot", P(0, 5.2, -1.5), P(0, 0.4, 1), (0.95, 0.88, 0.7), 3600, 50)
    bpy.ops.object.light_add(type="POINT", location=P(-3, 3.5, 2))
    fl = bpy.context.active_object
    fl.data.energy = 180
    fl.data.color = (0.55, 0.65, 0.55)
    for i in range(4):
        dad((22 - 3.2 + i * 2.0, 0, -2.6), i % 6, f"PissDad{i}", 1.0)


def build_bbq():
    def B(x, y, z):
        return (-22 + x, z, y)

    mesh("Patio", "cube", B(0, -0.05, 0), (14, 14, 0.1), mat("patio", (0.3, 0.24, 0.16), 0.9))
    mesh("House", "cube", B(0, 1.6, 6.2), (11, 0.45, 3.2), mat("house", (0.5, 0.38, 0.28)))
    mesh("Door", "cube", B(0, 1.1, 5.95), (1.1, 0.1, 2.1), M_WOOD)
    mesh("Porch", "sphere", B(0, 2.5, 5.85), (0.25, 0.25, 0.25), M_AMBER)
    mesh("Grill", "cylinder", B(0, 0.45, 0), (1.5, 1.5, 0.4), M_GRILL)
    mesh("Flame", "sphere", B(0, 1.2, 0), (0.75, 0.75, 1.1), M_FLAME)
    bpy.ops.object.light_add(type="POINT", location=B(0, 1.3, 0))
    fl = bpy.context.active_object
    fl.data.energy = 400
    fl.data.color = (1, 0.45, 0.1)
    mesh("Chair", "cube", B(2.8, 0.55, -1.2), (1.1, 0.12, 1.0), M_PLASTIC)
    mesh("Cooler", "cube", B(-3.2, 0.5, -2.2), (1.4, 0.9, 1.0), mat("cool", (0.18, 0.32, 0.65)))
    mesh("BushL", "sphere", B(-3.5, 0.6, 5.5), (1.4, 0.8, 1.1), M_GRASS)
    spot("PatioSpot", B(2, 5.2, -3.5), B(0, 0.8, 0), (1, 0.85, 0.65), 3200, 46)
    spot("PatioFill", B(-2, 3.8, -2), B(0, 0.6, 0), (1, 0.7, 0.45), 1000, 50)
    for i in range(4):
        ang = i * math.tau / 4
        dad((-22 + math.cos(ang) * 4.2, 0, math.sin(ang) * 4.2), i % 6, f"BBQDad{i}", 1.0)


def cam(loc, look, lens=32):
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
    for f in os.listdir(OUT):
        if f.endswith(".png"):
            os.remove(os.path.join(OUT, f))

    build_hub()
    build_baby_lane()
    build_piss()
    build_bbq()

    # Match Unity StageCameraDirector mounts (Buckshot intimacy / high-angle minis / baby pomp)
    shots = [
        # Lobby orbit seed — desk-centered intimacy
        (U(2.8, 1.55, -1.8), U(0.0, 1.45, 2.35), 38, "01-hub-lobby.png"),
        # Buckshot Host push
        ((1.15, 0.35, 1.55), (0.0, 2.85, 1.55), 36, "02-host-desk.png"),
        # Make Dad across-table
        (U(0.15, 1.35, -2.1), U(0.0, 1.4, 2.4), 40, "03-make-dad-hub.png"),
        # Fixed high-angle bathroom arena (all 4 dads readable)
        ((22.0, -7.1, 9.2), (22.0, 0.9, 0.35), 28, "04-pissing-contest.png"),
        # Bowling pomp baby
        ((3.4, 5.2, 2.55), (0.0, 9.8, 1.55), 34, "05-baby-showcase.png"),
        # High-angle BBQ
        ((-22.0, -7.6, 9.2), (-22.0, 0.4, 0.35), 28, "06-bbq-brawl.png"),
    ]
    for loc, look, lens, fn in shots:
        render(cam(loc, look, lens), fn)

    with open(os.path.join(OUT, "README.md"), "w") as f:
        f.write(
            """# Unity slice media — demo camera / HUD pass

Offline **Blender EEVEE** stills approximating Unity procedural kits after the **demo polish** pass (Buckshot hub cams, bathroom Pissing Contest, high-angle minis, baby pomp). **Not** Unity Game-view — no Unity license on this VM. HUD/chrome not burned into these stills.

| File | Beat |
|------|------|
| `01-hub-lobby.png` | Buckshot-table hub intimacy |
| `02-host-desk.png` | Host desk practicals / crushed blacks |
| `03-make-dad-hub.png` | Across-table Make Dad |
| `04-pissing-contest.png` | Grimy bathroom mini (ref 06 cues) · high angle |
| `05-baby-showcase.png` | Bowling-alley baby pomp |
| `06-bbq-brawl.png` | BBQ brawl · high angle |

Mitchell: Play `Bootstrap.unity` → **Saury → Capture 3D Slice Screenshots** for live Game-view PNGs (demo HUD included).
"""
        )
    print("DONE", file=sys.stderr)


if __name__ == "__main__":
    main()
