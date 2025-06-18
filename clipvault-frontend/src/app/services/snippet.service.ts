import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Snippet } from '../models/models';

export interface CreateSnippetRequest {
  title: string;
  description?: string;
  code: string;
  language: string;
  tagNames: string[];
  collectionId?: number;
}

export interface UpdateSnippetRequest {
  title?: string;
  description?: string;
  code?: string;
  language?: string;
  tagNames?: string[];
  collectionId?: number;
}

export interface SnippetSearchParams {
  query?: string;
  language?: string;
  tags?: string[];
  collectionId?: number;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root'
})
export class SnippetService {
  constructor(private apiService: ApiService) {}

  getAllSnippets(): Observable<Snippet[]> {
    return this.apiService.get<Snippet[]>('snippets');
  }

  getSnippetById(id: number): Observable<Snippet> {
    return this.apiService.get<Snippet>(`snippets/${id}`);
  }

  createSnippet(snippet: CreateSnippetRequest): Observable<Snippet> {
    return this.apiService.post<Snippet>('snippets', snippet);
  }

  updateSnippet(id: number, snippet: UpdateSnippetRequest): Observable<Snippet> {
    return this.apiService.put<Snippet>(`snippets/${id}`, snippet);
  }

  deleteSnippet(id: number): Observable<void> {
    return this.apiService.delete<void>(`snippets/${id}`);
  }

  searchSnippets(params: SnippetSearchParams): Observable<Snippet[]> {
    const queryParams = new URLSearchParams();
    
    if (params.query) queryParams.append('query', params.query);
    if (params.language) queryParams.append('language', params.language);    if (params.tags?.length) {
      params.tags.forEach(tag => queryParams.append('tags', tag));
    }
    if (params.collectionId) queryParams.append('collectionId', params.collectionId.toString());
    if (params.page) queryParams.append('page', params.page.toString());
    if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString());

    const queryString = queryParams.toString();
    const endpoint = queryString ? `snippets/search?${queryString}` : 'snippets/search';
    
    return this.apiService.get<Snippet[]>(endpoint);
  }

  getSnippetsByCollection(collectionId: number): Observable<Snippet[]> {
    return this.apiService.get<Snippet[]>(`collections/${collectionId}/snippets`);
  }

  getSnippetsByTag(tag: string): Observable<Snippet[]> {
    return this.apiService.get<Snippet[]>(`snippets/search?tags=${encodeURIComponent(tag)}`);
  }

  duplicateSnippet(id: number): Observable<Snippet> {
    return this.apiService.post<Snippet>(`snippets/${id}/duplicate`, {});
  }
}
