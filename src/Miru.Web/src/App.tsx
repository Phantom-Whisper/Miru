import './App.css'
import Sidebar from "./components/layout/Sidebar.tsx";
import Navbar from "./components/layout/Navbar.tsx";
import Hero from "./components/hero/Hero.tsx";
import Dashboard from "./pages/dashboard/Dashboard.tsx";

function App() {
    return (
        <div className="h-screen bg-black text-white flex overflow-hidden">
            <Sidebar />

            <main className="flex-1 min-w-0 flex flex-col">
                <Navbar />

                <div className="flex-1 overflow-y-auto scrollbar-hide">
                    <div className="px-8 py-6 space-y-10">
                        <Hero />
                        <Dashboard />
                    </div>
                </div>
            </main>
        </div>
    )
}

export default App