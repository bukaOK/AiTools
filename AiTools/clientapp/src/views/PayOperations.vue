<template>
    <v-layout row>
        <v-flex xs12 sm6>
            <v-card>
                <v-toolbar color="deep-purple" dark>
                    <v-toolbar-title>История операций</v-toolbar-title>
                    <v-spacer />
                </v-toolbar>
                <v-list two-line>
                    <v-list-tile v-for="item in pOperations" :key="item.title">
                        <v-list-tile-content>
                            <v-list-tile-title>{{ item.title }}</v-list-tile-title>
                            <v-list-tile-sub-title>{{ item.comment }}</v-list-tile-sub-title>
                        </v-list-tile-content>
                        <v-list-tile-action :class="item.sum > 0 ? 'green--text' : 'red--text'">
                            <v-list-tile-action-text class="display-1 font-weight-light" 
                                :class="item.sum > 0 ? 'green--text' : 'red--text'">
                                {{item.sum}}
                            </v-list-tile-action-text>
                        </v-list-tile-action>
                    </v-list-tile>
                </v-list>
                <v-divider />
                <div class="text-xs-center">
                    <v-btn round dark color="deep-purple" @click="loadOperations">Ещё</v-btn>
                </div>
            </v-card>
        </v-flex>
    </v-layout>
</template>
<script>
export default {
    data(){
        return {
            name: 'pay-operations',
            pOperations: [
                {
                    title: 'Перевод пользователю Vfqy',
                    comment: 'Спасиба',
                    sum: -60
                },
                {
                    title: 'Перевод пользователю ass',
                    comment: 'Спасиба',
                    sum: 60
                }
            ]
        }
    },
    methods: {
        loadOperations(){

        }
    }
}
</script>

