import { Renderer2 } from '@angular/core';

export abstract class PluggableScriptComponent {
  protected constructor(private readonly _renderer: Renderer2) {
  }

  protected includeScript(id: string, text: string): void {
    // script fire custom 'request' event on document. This event catching and handling in the material component.
    const script = this._renderer.createElement('script');

    const rootNode = this.getRootNode();
    if (rootNode.getElementById(id)) {
      return;
    }

    this._renderer.setProperty(script, 'id', id);
    this._renderer.setProperty(script, 'text', text);
    this._renderer.appendChild(rootNode, script);
  }

  protected abstract getRootNode(): DocumentFragment;
}
