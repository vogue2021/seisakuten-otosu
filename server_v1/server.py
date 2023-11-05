import os
import wave
import numpy as np
from flask import Flask, request, send_from_directory, jsonify
from flask_uploads import UploadSet, configure_uploads, AUDIO
from scipy.io import wavfile
from scipy import signal
from matplotlib import pyplot as plt
from test import voice_png
from werkzeug.utils import secure_filename
from werkzeug.datastructures import  FileStorage


app = Flask(__name__)

audios = UploadSet('audios', AUDIO)
app.config['UPLOADED_AUDIOS_DEST'] = 'uploads'
configure_uploads(app, audios)

images = UploadSet('images', ('png',))
app.config['UPLOADED_IMAGES_DEST'] = 'images'
configure_uploads(app, images)

def audio_to_image(audio_path):
    voice_png(audio_path)
@app.route('/upload', methods=['POST'])
def upload():
    if 'audio' not in request.files:
        return 'No audio file uploaded', 400
    
    audio_file = request.files['audio']
    audio_filename = audios.save(audio_file)
    
    audio_path = os.path.join(app.config['UPLOADED_AUDIOS_DEST'], audio_filename)
    image_filename = os.path.splitext(audio_filename)[0] + '.png'
    # image_path = os.path.join(app.config['UPLOADED_IMAGES_DEST'], image_filename)
    
    audio_to_image(audio_path)
    
    return jsonify({
        'audio_filename': audio_filename,
        'image_filename': image_filename
    })

@app.route('/images/<filename>')
def send_images(filename):
    return send_from_directory(app.config['UPLOADED_IMAGES_DEST'], filename)

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0')
