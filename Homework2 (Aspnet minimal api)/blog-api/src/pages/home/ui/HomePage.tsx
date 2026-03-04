import type { RootState } from '@/app'
import { CategorySelect } from '@/features/category'
import { PostCard, type Post } from '@/features/post'
import { useState } from 'react'
import { useSelector } from 'react-redux'

export const HomePage = () => {
    const [page, setPage] = useState(1)
    const { posts } = useSelector((state: RootState) => state.posts)

    return (
        <main className="container mx-auto py-8 space-y-6">
            <header className="flex flex-col md:flex-row gap-4 justify-between items-center">
                <h1 className="text-3xl font-bold tracking-tight">Feed</h1>

                <div className="flex w-full md:w-auto items-center gap-3">
                    <PostSearch />
                    <CategorySelect />
                </div>
            </header>

            <section className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {posts?.map((post: Post) => (
                    <PostCard key={post.id} {...post} />
                ))}
            </section>

            <div id="scroll-threshold" className="h-10 w-full" />
        </main>
    )
}
