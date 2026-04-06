import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class QuizSoundService {
  private _soundEnabled = true;
  private _audioContext: AudioContext | null = null;
  private _correctAudio: HTMLAudioElement | null = null;

  setSoundEnabled(enabled: boolean): void {
    this._soundEnabled = enabled;
  }

  playCorrect(): void {
    if (!this._soundEnabled) {
      return;
    }
    this._playCorrectAudio();
  }

  playIncorrect(): void {
    if (!this._soundEnabled) {
      return;
    }
    this._playDescendingBuzz();
  }

  private _playCorrectAudio(): void {
    try {
      if (!this._correctAudio) {
        this._correctAudio = new Audio('assets/sounds/Biobrain-correct-answer.mp3');
      }
      this._correctAudio.currentTime = 0;
      this._correctAudio.play();
    } catch {
      // Audio not supported — silently ignore
    }
  }

  private _playDescendingBuzz(): void {
    try {
      const ctx = this._getAudioContext();
      if (!ctx) {
        return;
      }

      const osc = ctx.createOscillator();
      const gain = ctx.createGain();
      osc.connect(gain);
      gain.connect(ctx.destination);

      osc.type = 'square';
      osc.frequency.setValueAtTime(330, ctx.currentTime); // E4
      osc.frequency.linearRampToValueAtTime(262, ctx.currentTime + 0.3); // C4

      gain.gain.setValueAtTime(0.15, ctx.currentTime);
      gain.gain.linearRampToValueAtTime(0, ctx.currentTime + 0.3);

      osc.start(ctx.currentTime);
      osc.stop(ctx.currentTime + 0.3);
    } catch {
      // Audio not supported — silently ignore
    }
  }

  private _getAudioContext(): AudioContext | null {
    if (!this._audioContext) {
      const AudioContextClass = window.AudioContext || (window as any).webkitAudioContext;
      if (!AudioContextClass) {
        return null;
      }
      this._audioContext = new AudioContextClass();
    }
    return this._audioContext;
  }
}
