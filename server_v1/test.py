from o_to_t import onnseie
from PIL import Image, ImageDraw, ImageFont
import librosa
import numpy as np
import colorsys
import random


# -------------------------------コーディング------------------------------
# 将 HSL 值转换为 RGB 值
def hsl_to_rgb(h, s, l):
    r, g, b = colorsys.hls_to_rgb(h/360, l/100, s/100)
    return int(r * 255), int(g * 255), int(b * 255)

# 周波数（音高）
def fft(file):
    y, sr = librosa.load(file)
    fft_data = librosa.core.fft_frequencies(sr=sr)
    fft_data = np.abs(fft_data)
    return np.mean(fft_data)

# 音量
def volume(file):
    y, sr = librosa.load(file)
    volume = librosa.feature.rms(y=y)
    volume = np.abs(volume)
    return np.mean(np.squeeze(volume))

# speed
def s_speed(file):
    y, sr = librosa.load(file)
    a = onnseie(file, '_', '_')
    sec = len(y)/sr
    text_len = len(a.speech_to_text())
    speed = text_len/sec
    return round(speed)

# ファントデータベース（ルールに従い、何の感情は何番目〜何番目の間にランダムに抽出する）
font_database = ["fontList/HachiMaruPop-Regular.ttf",
                 "fontList/Humour-Normal.otf",
                 "fontList/Humour-Normal.ttf",
                 "fontList/Humour-Original.otf",
                 "fontList/Humour-Original.ttf",
                 "fontList/Kinkakuji-Normal.otf",
                 "fontList/Kinkakuji-Normal.ttf",
                 "fontList/Mamelon-3-Hi-Regular.otf",
                 "fontList/Mamelon-3.5-Hi-Regular.otf",
                 "fontList/Mamelon-4-Hi-Regular.otf",
                 "fontList/Mamelon-5-Hi-Regular.otf",
                 "fontList/MochiyPopOne-Regular.ttf",
                 "fontList/NotoSansJP-VariableFont_wght.ttf",
                 "fontList/RampartOne-Regular.ttf",
                 "fontList/Togalite-Black.otf",
                 "fontList/Togalite-Bold.otf",
                 "fontList/Togalite-Heavy.otf",
                 "fontList/Togalite-Light.otf",
                 "fontList/Togalite-Medium.otf",
                 "fontList/Togalite-Regular.otf",
                 "fontList/Togalite-Thin.otf"]

def Font(speech_file):
    tp = s_speed(speech_file)
    if tp < 1:
        font = font_database[random.randint(0, 3)]
    elif tp < 2 and tp >= 1:
        font = font_database[random.randint(3, 6)]
    elif tp < 3 and tp >= 2:
        font = font_database[random.randint(6, 10)]
    elif tp < 4 and tp >= 3:
        font = font_database[random.randint(10, 15)]
    elif tp < 5 and tp >= 4:
        font = font_database[random.randint(15, 18)]
    else:
        font = font_database[random.randint(18, 21)]
    return font

def Font_size(speech_file):
    vo = volume(speech_file)
    if vo < 0.05:
        font_size = 20
    elif vo < 0.1 and vo >= 0.05:
        font_size = 40
    else:
        font_size = 60
    return font_size

def Font_color(speech_file):
    ft = fft(speech_file)
    if ft < 4500:
        h = random.randint(0, 100)
        s = random.randint(0, 100)
        l = random.randint(80, 100)
    elif ft < 5000 and ft >= 4500:
        h = random.randint(100, 150)
        s = random.randint(0, 100)
        l = random.randint(80, 90)
    elif ft < 5300 and ft >= 5000:
        h = random.randint(70, 290)
        s = random.randint(0, 100)
        l = random.randint(50, 70)
    elif ft < 5500 and ft >= 5300:
        h = random.randint(50, 240)
        s = random.randint(0, 100)
        l = random.randint(70, 80)
    elif ft < 5600 and ft >= 5500:
        h = random.randint(240, 280)
        s = random.randint(0, 100)
        l = random.randint(60, 90)
    elif ft < 5700 and ft >= 5600:
        h = random.randint(280, 320)
        s = random.randint(0, 100)
        l = random.randint(10, 20)
    else:
        h = random.randint(200, 360)
        s = random.randint(0, 100)
        l = random.randint(0, 10)
    return hsl_to_rgb(h, s, l)


def voice_png(speech_file):
    font = Font(speech_file)
    print("finish font")
    font_size = Font_size(speech_file)
    print("finish font_size")
    font_color = Font_color(speech_file)
    print("finish font_color")
    FontStyle = ImageFont.truetype(font, font_size)
    print("finish fontstyle")
    test = onnseie(speech_file, FontStyle, font_color)
    test.split_text()