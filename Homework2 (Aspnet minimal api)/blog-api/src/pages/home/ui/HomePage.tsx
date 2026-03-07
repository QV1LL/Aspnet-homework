import { useEffect } from 'react'
import { useSelector, useDispatch } from 'react-redux'
import { CategorySelect } from '@/features/category'
import { PostCard, PostSearch, type Post } from '@/features/post'
import { selectAllPosts, selectPostsMeta, setPage } from '@/features/post'
import { fetchPosts } from '@/features/post'
import { Button } from '@/components/ui/button'
import { Loader2, ChevronLeft, ChevronRight } from 'lucide-react'
import type { AppDispatch } from '@/app'

export const HomePage = () => {
    const dispatch = useDispatch<AppDispatch>()
    const posts = useSelector(selectAllPosts)
    const { status, totalCount, currentPage, filters } = useSelector(selectPostsMeta)

    const pageSize = 9
    const totalPages = Math.ceil(totalCount / pageSize)
    const hasMore = currentPage < totalPages

    useEffect(() => {
        if (status === 'loading') return

        const expectedCount = currentPage * pageSize
        const isCurrentPageLoaded = posts.length >= expectedCount

        if (isCurrentPageLoaded && status === 'idle') return

        dispatch(
            fetchPosts({
                page: currentPage,
                size: pageSize,
                search: filters.search,
                category: filters.category,
            }),
        )
    }, [dispatch, currentPage, filters.search, filters.category])

    const handleNextPage = () => {
        if (hasMore && status !== 'loading') {
            dispatch(setPage(currentPage + 1))
        }
    }

    const handlePrevPage = () => {
        if (currentPage > 1 && status !== 'loading') {
            dispatch(setPage(currentPage - 1))
        }
    }

    const isInitialLoading = status === 'loading' && posts.length === 0

    return (
        <main className="container mx-auto py-8 space-y-8">
            <header className="flex flex-col md:flex-row gap-4 justify-between items-center">
                <h1 className="text-4xl font-extrabold tracking-tight">Post Feed</h1>
                <div className="flex w-full md:w-auto items-center gap-3">
                    <PostSearch />
                    <CategorySelect />
                </div>
            </header>

            {isInitialLoading ? (
                <div className="flex justify-center py-20">
                    <Loader2 className="h-10 w-10 animate-spin text-primary" />
                </div>
            ) : (
                <>
                    <section className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {posts.slice((currentPage - 1) * pageSize, currentPage * pageSize).map((post: Post) => (
                            <PostCard key={post.id} {...post} />
                        ))}
                    </section>

                    {posts.length === 0 && status === 'idle' && (
                        <div className="text-center py-20 text-muted-foreground">
                            No posts found matching your criteria.
                        </div>
                    )}
                </>
            )}

            <footer className="flex items-center justify-center gap-4 pt-10 border-t">
                <Button
                    variant="outline"
                    size="sm"
                    onClick={handlePrevPage}
                    disabled={currentPage === 1 || status === 'loading'}
                >
                    <ChevronLeft className="h-4 w-4 mr-2" />
                    Previous
                </Button>

                <div className="flex items-center gap-1">
                    <span className="text-sm font-medium">Page {currentPage}</span>
                    <span className="text-sm text-muted-foreground">of {totalPages || 1}</span>
                </div>

                <Button
                    variant="outline"
                    size="sm"
                    onClick={handleNextPage}
                    disabled={!hasMore || status === 'loading'}
                >
                    Next
                    <ChevronRight className="h-4 w-4 ml-2" />
                </Button>
            </footer>
        </main>
    )
}
