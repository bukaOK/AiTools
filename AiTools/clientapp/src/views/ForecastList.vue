<template>
    <v-card>
        <v-card-title>
            <h3 class="title">Прогнозные модели</h3>
        </v-card-title>
        <v-card-text>
            <v-list>
                <v-list-tile v-for="res in results" :key="res.value"
                    :to="`/forecasts/edit/${res.value}`">
                    <v-list-tile-content>
                        <v-list-tile-title>{{res.text}}</v-list-tile-title>
                    </v-list-tile-content>
                    <v-list-tile-action>
                        <v-btn icon ripple @click.prevent="removeResult(res.value)">
                            <v-icon color="grey">delete</v-icon>
                        </v-btn>
                    </v-list-tile-action>
                </v-list-tile>
            </v-list>
        </v-card-text>
    </v-card>
</template>
<script>
import { mapState } from 'vuex';

export default {
    mounted(){
        this.api.get('/forecast/getresults/')
            .then(resp => {
                this.results = resp.data.map(src => ({
                    text: src.text,
                    value: src.value
                }))
            })
    },
    data(){
        return {
            results: []
        }
    },
    computed: {
        ...mapState([
            'api'
        ])
    },
    methods: {
        removeResult(id){
            this.api.delete('/forecast/removeresult/' + id)
                .then(() => {
                    const ind = this.results.findIndex(x => x.value === id)
                    this.results.splice(ind, 1)
                })
        }
    }
}
</script>
