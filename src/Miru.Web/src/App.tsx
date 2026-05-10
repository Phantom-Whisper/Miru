import './App.css'
import Sidebar from "./components/layout/Sidebar.tsx";
import Navbar from "./components/layout/Navbar.tsx";
import Hero from "./components/Hero/Hero.tsx";

function App() {
    return (
        <div className="h-screen bg-black text-white flex overflow-hidden">
            <Sidebar />

            <main className="flex-1 flex flex-col">
                <Navbar />

                <div className="flex-1 overflow-y-auto px-8 py-6">
                    <Hero />
                </div>
            </main>
        </div>
    )
}

export default App