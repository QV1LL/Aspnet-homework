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
