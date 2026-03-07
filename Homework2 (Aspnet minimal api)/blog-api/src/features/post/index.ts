export { PostCard } from './ui/PostCard'
export { PostSearch } from './ui/PostSearch'
export { type Post } from './model/types'
export {
    postsReducer,
    selectAllPosts,
    selectPostsMeta,
    setFilters,
    setPage,
    postAdded,
    postUpdated,
    postRemoved,
} from './model/slice'
export { fetchPosts, createPost, updatePost, deletePost } from './model/actions'
export { PostForm } from './ui/PostForm'
