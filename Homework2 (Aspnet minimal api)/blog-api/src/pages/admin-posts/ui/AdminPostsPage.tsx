import { useEffect, useState } from 'react'
import { useSelector, useDispatch } from 'react-redux'
import { PostForm, selectAllPosts, selectPostsMeta } from '@/features/post'
import { fetchPosts, deletePost } from '@/features/post'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import { Edit, Trash2, X, Plus } from 'lucide-react'
import type { AppDispatch } from '@/app'
import type { Post } from '@/features/post/model/types'

export const AdminPostsPage = () => {
    const dispatch = useDispatch<AppDispatch>()
    const posts = useSelector(selectAllPosts)
    const { currentPage } = useSelector(selectPostsMeta)
    const [isFormOpen, setIsFormOpen] = useState(false)
    const [selectedPost, setSelectedPost] = useState<Post | undefined>(undefined)

    useEffect(() => {
        dispatch(fetchPosts({ page: currentPage, size: 10 }))
    }, [dispatch, currentPage])

    const handleEdit = (post: Post) => {
        setSelectedPost(post)
        setIsFormOpen(true)
    }

    const handleCreate = () => {
        setSelectedPost(undefined)
        setIsFormOpen(true)
    }

    return (
        <div className="p-6 space-y-6">
            <div className="flex justify-between items-center">
                <h1 className="text-3xl font-bold">Posts</h1>
                <Button size="sm" onClick={handleCreate}>
                    <Plus className="w-4 h-4 mr-2" /> New Post
                </Button>
            </div>

            {isFormOpen && (
                <div className="fixed inset-0 z-50 bg-black/50 flex items-center justify-center p-4 backdrop-blur-sm">
                    <div className="bg-white rounded-xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto relative">
                        <div className="sticky top-0 bg-white p-4 border-b flex justify-between items-center z-10">
                            <h2 className="text-xl font-bold">{selectedPost ? 'Edit Post' : 'Create New Post'}</h2>
                            <Button variant="ghost" size="icon" onClick={() => setIsFormOpen(false)}>
                                <X className="h-5 w-5" />
                            </Button>
                        </div>
                        <div className="p-6">
                            <PostForm post={selectedPost} onSuccess={() => setIsFormOpen(false)} />
                        </div>
                    </div>
                </div>
            )}

            <div className="border rounded-lg bg-white shadow-sm overflow-hidden">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Title</TableHead>
                            <TableHead>Categories</TableHead>
                            <TableHead className="text-right">Actions</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {posts.map((post) => (
                            <TableRow key={post.id} className="hover:bg-muted/50 transition-colors">
                                <TableCell className="font-medium max-w-xs truncate">{post.title}</TableCell>
                                <TableCell>
                                    <div className="flex flex-wrap gap-1">
                                        {post.categories.map((c) => (
                                            <span key={c.id} className="text-[10px] px-1.5 py-0.5 rounded bg-secondary">
                                                {c.name}
                                            </span>
                                        ))}
                                    </div>
                                </TableCell>
                                <TableCell className="text-right space-x-1">
                                    <Button variant="ghost" size="icon" onClick={() => handleEdit(post)}>
                                        <Edit className="w-4 h-4" />
                                    </Button>
                                    <Button
                                        variant="ghost"
                                        size="icon"
                                        onClick={() => confirm('Delete post?') && dispatch(deletePost(post.id))}
                                    >
                                        <Trash2 className="w-4 h-4 text-destructive" />
                                    </Button>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </div>
        </div>
    )
}
