import { Routes } from '@angular/router';
import { SnippetGallery } from './components/snippet-gallery/snippet-gallery';
import { Component } from '@angular/core';

@Component({
  standalone: true,
  template: '<h2>Login Page Placeholder</h2>'
})
export class LoginPlaceholder {}

@Component({
  standalone: true,
  template: '<h2>Register Page Placeholder</h2>'
})
export class RegisterPlaceholder {}

export const routes: Routes = [
  { path: '', component: SnippetGallery },
  { path: 'login', component: LoginPlaceholder },
  { path: 'register', component: RegisterPlaceholder },
  { path: 'gallery', component: SnippetGallery }
];
