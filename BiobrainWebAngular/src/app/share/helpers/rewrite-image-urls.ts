import { environment } from '../../../environments/environment';

/**
 * Rewrites relative image paths in HTML content to use the API backend URL.
 *
 * DB content contains paths like `src="Images/UUID.png"` or `src="user-guide-images/file.png"`.
 * On BinaryLane these resolve correctly but on Firebase hosting they 404.
 * This function prepends the API URL so images are fetched through the backend proxy.
 */
export function rewriteImageUrls(html: string): string {
  if (!html) return html;

  // staticUrl takes priority for images (e.g. prod server serving static files)
  // Falls back to apiUrl, then empty (local dev)
  const baseUrl = (environment as any).staticUrl || environment.apiUrl;
  if (!baseUrl) return html; // dev mode — images served locally

  // Match src="Images/..." or src='Images/...' (case-insensitive for Images)
  // Also handles src="/Images/..." with leading slash
  html = html.replace(
    /(<img\s[^>]*?src\s*=\s*["'])(\/?)((?:Images|images)\/)/gi,
    `$1${baseUrl}/$3`
  );

  // Match src="user-guide-images/..." or src="/user-guide-images/..."
  html = html.replace(
    /(<img\s[^>]*?src\s*=\s*["'])(\/?)(user-guide-images\/)/gi,
    `$1${baseUrl}/$3`
  );

  return html;
}
