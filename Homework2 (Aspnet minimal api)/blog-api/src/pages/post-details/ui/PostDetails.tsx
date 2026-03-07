import { useParams, useNavigate } from 'react-router'
import { useSelector } from 'react-redux'
import { ChevronLeft, Calendar, Tag } from 'lucide-react'
import { format } from 'date-fns'
import { selectPostById, selectPostsMeta } from '@/features/post/model/slice'
import type { RootState } from '@/app'
import { PostDetailsSkeleton } from './PostDetailsSkeleton'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import type { Category } from '@/features/category'

export const PostDetailsPage = () => {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()

    const post = useSelector((state: RootState) => selectPostById(state, id || ''))
    const { status } = useSelector(selectPostsMeta)

    if (status === 'loading' && !post) {
        return <PostDetailsSkeleton />
    }

    if (!post) {
        return (
            <div className="container flex flex-col items-center justify-center py-20 text-center">
                <h2 className="text-2xl font-bold">Post not found</h2>
                <Button variant="link" onClick={() => navigate('/')}>
                    Return to feed
                </Button>
            </div>
        )
    }

    return (
        <article className="container max-w-3xl mx-auto py-10 space-y-8">
            <Button variant="ghost" size="sm" className="mb-4 -ml-4 text-muted-foreground" onClick={() => navigate(-1)}>
                <ChevronLeft className="mr-2 h-4 w-4" />
                Back to posts
            </Button>

            <header className="space-y-4">
                <div className="flex flex-wrap gap-2">
                    {post.categories.map((category: Category) => (
                        <Badge key={category.id} variant="secondary">
                            {category.name}
                        </Badge>
                    ))}
                </div>

                <h1 className="text-4xl md:text-5xl font-extrabold tracking-tight lg:text-6xl">{post.title}</h1>

                <div className="flex items-center text-sm text-muted-foreground gap-4">
                    <div className="flex items-center">
                        <Calendar className="mr-1 h-4 w-4" />
                        {format(new Date(post.createdAt), 'MMMM d, yyyy')}
                    </div>
                    {post.slugs.length > 0 && (
                        <div className="flex items-center">
                            <Tag className="mr-1 h-4 w-4" />
                            {post.slugs[0]}
                        </div>
                    )}
                </div>
            </header>

            {post.imageUrl && (
                <div className="aspect-video overflow-hidden rounded-xl border bg-muted">
                    <img
                        src={post.imageUrl}
                        alt={post.title}
                        className="h-full w-full object-cover transition-all hover:scale-105"
                    />
                </div>
            )}

            <section className="prose prose-stone dark:prose-invert max-w-none leading-7 [&:not(:first-child)]:mt-6">
                {post.content.split('\n').map((paragraph, index) => (
                    <p key={index} className="mb-4 text-lg">
                        {paragraph}
                    </p>
                ))}
            </section>
        </article>
    )
}
