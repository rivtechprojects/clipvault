export interface Snippet {
  id: number;
  title: string;
  code: string;
  language: string;
  tags: string[];
  description?: string;
  isDeleted: boolean;
}

export interface Collection {
  id: number;
  name: string;
  snippets: Snippet[];
  parentCollectionId: number | null;
  subCollections: Collection[];
  isDeleted: boolean;
}