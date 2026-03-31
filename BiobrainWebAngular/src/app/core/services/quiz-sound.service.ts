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
    this._playTone([523.25, 659.25], 0.2);
  }

  playIncorrect(): void {
    if (!this._soundEnabled) {
      return;
    }
    this._playTone([659.25, 523.25], 0.2);
  }

  private _playTone(frequencies: [number, number], duration: number): void {
    try {
      const ctx = this._getAudioContext();
      if (!ctx) {
        return;
      }

      const oscillator = ctx.createOscillator();
      const gainNode = ctx.createGain();

      oscillator.connect(gainNode);
      gainNode.connect(ctx.destination);

      oscillator.type = 'sine';
      oscillator.frequency.setValueAtTime(frequencies[0], ctx.currentTime);
      oscillator.frequency.linearRampToValueAtTime(frequencies[1], ctx.currentTime + duration);

      gainNode.gain.setValueAtTime(0.3, ctx.currentTime);
      gainNode.gain.linearRampToValueAtTime(0, ctx.currentTime + duration);

      oscillator.start(ctx.currentTime);
      oscillator.stop(ctx.currentTime + duration);
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
