import { Routes } from '@angular/router';
import { SnippetGallery } from './components/snippet-gallery/snippet-gallery';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/gallery', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { 
    path: 'gallery', 
    component: SnippetGallery,
    canActivate: [AuthGuard]
  },
  { path: '**', redirectTo: '/gallery' }
];
