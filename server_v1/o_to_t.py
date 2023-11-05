import speech_recognition as sr
from PIL import Image, ImageDraw, ImageFont

# ファント様式：font
# ファントサイズ：font_size
# ファント色：font_color

class onnseie:
    def __init__(self, speech_file, font, font_color):
        self.speech_file = speech_file
        self.font        = font
        self.font_color  = font_color
        # self.font1 = ImageFont.truetype("/System/Library/Fonts/Supplemental/Andale Mono.ttf", 40)
        # self.font2 = ImageFont.truetype("/System/Library/Fonts/ヒラギノ角ゴシック W9.ttc", 40)

    def speech_to_text(self):
        r = sr.Recognizer()
        with sr.AudioFile(self.speech_file) as source:
            audio = r.record(source)
        texts = r.recognize_google(audio, language='ja-JP')
        return texts
        # print(texts)

    def split_text(self):
        # RGB, 画像サイズ, 背景色を設定
        im = Image.new("RGBA", (256, 256), (0, 0, 0, 0))

        draw = ImageDraw.Draw(im)

        # PCローカルのフォントへのパスと、フォントサイズを指定
        # font = ImageFont.truetype("/System/Library/Fonts/Supplemental/Andale Mono.ttf", 40)
        # font = ImageFont.truetype("/System/Library/Fonts/ヒラギノ角ゴシック W9.ttc", 40)

        # 文字描画の初期位置（画像左上からx, yだけ離れた位置）
        x = 20
        y = 200

        texts = self.speech_to_text()
        # draw.text((x, y), texts, fill=(200, 200, 146), font=self.font2)
        draw.text((x, y), texts, fill=self.font_color, font=self.font)

        # ファイルに出力
        im.save("images/processed_image.png")