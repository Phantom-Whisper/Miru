import {FormattedMessage} from "react-intl";

function Hero() {
    return (
        <section>
            <div className="mb-8">
                <h2 className="text-4xl font-bold mb-2">
                    <FormattedMessage
                    id="hero.title" />
                </h2>

                <p className="text-zinc-400 text-lg">
                    <FormattedMessage
                    id="hero.subtitle" />
                </p>
            </div>
        </section>
    )
}

export default Hero