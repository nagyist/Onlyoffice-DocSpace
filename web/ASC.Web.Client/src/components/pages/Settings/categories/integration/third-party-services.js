import React from "react";
import { consumers } from "./sub-components/consumers";
import { Box, Text } from "asc-web-components";
import ConsumerItem from "./sub-components/consumerItem";

class ThirdPartyServices extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            consumers: consumers,
            selectedConsumer: -1,
            dialogVisible: false
        }
    }

    onToggleClick = () => {
        this.setState({
            dialogVisible: true
        })
    }

    onModalClose = () => {
        this.setState({
            dialogVisible: false
        })
    }

    titleDescription = "Ключи авторизации позволяют подключить портал ONLYOFFICE к сторонним сервисам, таким как Twitter, Facebook, Dropbox и т.д. Подключите портал к Facebook, Twitter или Linkedin, если Вы не хотите каждый раз при входе вводить свои учетные данные на портале. Привяжите портал к таким сервисам, как Dropbox, OneDrive и т.д. чтобы перенести документы из всех этих хранилищ в модуль Документы ONLYOFFICE."

    render() {

        const { consumers, selectedConsumer, dialogVisible } = this.state;
        const { titleDescription, onModalClose, onToggleClick } = this;

        return (
            <>
                <Box displayProp="flex" flexDirection="column">
                    <Box marginProp="10px">
                        <Text>{titleDescription}</Text>
                    </Box>
                    <Box displayProp="flex">
                        <Box displayProp="flex" widthProp="100%">
                            {consumers
                                .map((consumer, i) =>
                                    <ConsumerItem
                                        key={i}
                                        name={consumer.name}
                                        description={consumer.description}

                                        consumers={consumers}
                                        dialogVisible={dialogVisible}
                                        selectedConsumer={selectedConsumer}

                                        onModalClose={onModalClose}
                                        onToggleClick={onToggleClick}
                                    />
                                )}
                        </Box>
                    </Box>
                </Box>
            </>
        )
    }
}


export default ThirdPartyServices;