import type { Category } from '@/features/category'

export interface Post {
    id: string
    title: string
    content: string
    imageUrl: string
    createdAt: Date
    slugs: string[]
    categories: Category[]
}

export interface CreatePost {
    title: string
    content: string
    slugs: string[]
    categories: string[]
}

export interface UpdatePost extends CreatePost {
    id: string
}
