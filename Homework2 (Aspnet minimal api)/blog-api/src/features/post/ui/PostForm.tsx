import { useForm } from 'react-hook-form'
import { useSelector, useDispatch } from 'react-redux'
import { createPost, updatePost } from '../model/actions'
import { fetchCategories, selectAllCategories, type Category } from '@/features/category'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import type { Post, CreatePost } from '../model/types'
import type { AppDispatch, RootState } from '@/app'
import { useEffect } from 'react'

interface Props {
    post?: Post
    onSuccess: () => void
}

export const PostForm = ({ post, onSuccess }: Props) => {
    const dispatch = useDispatch<AppDispatch>()
    const categories = useSelector(selectAllCategories)
    const categoryStatus = useSelector((state: RootState) => state.categories.status)

    useEffect(() => {
        if (categoryStatus === 'idle') {
            dispatch(fetchCategories())
        }
    }, [categoryStatus, dispatch])

    const { register, handleSubmit } = useForm<CreatePost>({
        defaultValues: post
            ? {
                  title: post.title,
                  content: post.content,
                  slugs: post.slugs,
                  categories: post.categories.map((c: Category) => c.id),
              }
            : {
                  slugs: [],
                  categories: [],
              },
    })

    const onSubmit = async (data: CreatePost) => {
        if (post) {
            await dispatch(updatePost({ ...data, id: post.id }))
        } else {
            await dispatch(createPost(data))
        }
        onSuccess()
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 p-4 bg-white border rounded-lg">
            <div>
                <label className="text-sm font-medium">Title</label>
                <Input {...register('title', { required: true })} placeholder="Post title" />
            </div>

            <div>
                <label className="text-sm font-medium">Content</label>
                <Textarea {...register('content', { required: true })} placeholder="Write something..." rows={5} />
            </div>

            <div>
                <label className="text-sm font-medium">Categories (Hold Ctrl to select multiple)</label>
                <select multiple {...register('categories')} className="w-full border rounded-md p-2 h-32">
                    {categories.map((c) => (
                        <option key={c.id} value={c.id}>
                            {c.name}
                        </option>
                    ))}
                </select>
            </div>

            <Button type="submit" className="w-full">
                {post ? 'Update Post' : 'Create Post'}
            </Button>
        </form>
    )
}
