import { Component, input } from '@angular/core';

/**
 * Centered auth shell: logo, subtitle, framed card (projected content) and the
 * copyright footer. Shared by the login and register screens.
 */
@Component({
  selector: 'app-auth-card',
  templateUrl: './auth-card.html',
})
export class AuthCard {
  subtitle = input('');
}
