import { environment } from '../../environments/environment';

export class AuthConfig{
    public static get AuthAPIPath(): string {
        const prefix = environment.apiUrl ? environment.apiUrl : '';
        return `${prefix}/api/connect/token`;
    }
}