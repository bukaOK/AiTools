export default {
    parseErrors(errors) {
        const parsed = []
        for (let key in errors) {
            for (let err of errors[key]) {
                parsed.push(err)
            }
        }
        return parsed
    }
}