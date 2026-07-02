import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { CategoryService } from '../../core/services/category.service';

@Component({
	selector: 'app-sidebar',
	standalone: true,
	imports: [RouterLink, RouterLinkActive],
	templateUrl: './sidebar.html',
	styleUrl: './sidebar.scss'
})
export class Sidebar {
	private readonly categoryService = inject(CategoryService);

	readonly categories = toSignal(this.categoryService.getAll(), { initialValue: [] });
}