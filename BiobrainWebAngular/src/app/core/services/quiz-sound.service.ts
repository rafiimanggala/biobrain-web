import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class QuizSoundService {
  private _soundEnabled = true;
  private _audioContext: AudioContext | null = null;

  setSoundEnabled(enabled: boolean): void {
    this._soundEnabled = enabled;
  }

  playCorrect(): void {
    if (!this._soundEnabled) {
      return;
    }
    this._playArpeggio();
  }

  playIncorrect(): void {
    if (!this._soundEnabled) {
      return;
    }
    this._playDescendingBuzz();
  }

  private _playArpeggio(): void {
    try {
      const ctx = this._getAudioContext();
      if (!ctx) {
        return;
      }

      const notes = [523, 659, 784]; // C5, E5, G5
      const totalDuration = 0.35;
      const noteDuration = totalDuration / notes.length;

      notes.forEach((freq, i) => {
        const osc = ctx.createOscillator();
        const gain = ctx.createGain();
        osc.connect(gain);
        gain.connect(ctx.destination);

        osc.type = 'sine';
        osc.frequency.setValueAtTime(freq, ctx.currentTime);

        const start = ctx.currentTime + i * noteDuration;
        gain.gain.setValueAtTime(0, start);
        gain.gain.linearRampToValueAtTime(0.3, start + 0.02);
        gain.gain.linearRampToValueAtTime(0, start + noteDuration);

        osc.start(start);
        osc.stop(start + noteDuration);
      });
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
