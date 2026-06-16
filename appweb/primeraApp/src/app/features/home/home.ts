import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Auth } from '../../core/services/auth';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  private auth = inject(Auth);

  // El botón principal del hero apunta a la primera sección accesible según los
  // permisos del usuario, para no enviarlo nunca a una página sin acceso.
  primaryAction = computed(() => {
    if (this.auth.hasAnyPermission(['ListOrders', 'ViewOrderDetail'])) {
      return { label: 'View orders', link: '/orders' };
    }
    if (this.auth.hasPermission('ViewProducts')) {
      return { label: 'View products', link: '/products' };
    }
    if (this.auth.hasPermission('ViewPromotions')) {
      return { label: 'View promotions', link: '/promotions' };
    }
    return { label: 'Go to my profile', link: '/profile' };
  });
}
