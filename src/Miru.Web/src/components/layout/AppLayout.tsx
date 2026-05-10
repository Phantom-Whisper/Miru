import Sidebar from "./Sidebar.tsx";
import Navbar from "./Navbar.tsx";

function AppLayout(){
    return (
        <div className="flex h-screen w-screen bg-zinc-950 text-white">
            <Sidebar />
            <Navbar />
        </div>

    )
}

export default AppLayout