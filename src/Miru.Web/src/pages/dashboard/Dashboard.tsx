import ContentRow from "../../components/content/ContentRow.tsx";

function Dashboard() {
    return (
        <div>
            <ContentRow
                title="Watching"
            />

            <ContentRow
                title="To Watch"
            />

            <ContentRow
                title="Your last watched"
            />
        </div>
    )
}

export default Dashboard;