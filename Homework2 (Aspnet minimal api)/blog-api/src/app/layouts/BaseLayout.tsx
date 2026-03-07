import { Outlet } from 'react-router'
import { Provider } from 'react-redux'
import { store } from '../store/store'

export const BaseLayout = () => {
    return (
        <Provider store={store}>
            <main className="container mx-auto max-w-[1200px] px-4 py-8 md:px-8 lg:py-12">
                <div className="animate-in fade-in slide-in-from-bottom-2 duration-500">
                    <Outlet />
                </div>
            </main>
        </Provider>
    )
}
