<script>
import {Bar} from 'vue-chartjs'

export default {
    extends: Bar,
    props: ['datacollection'],
    data: () => ({
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    }),
    mounted(){
        this.renderChart(this.datacollection, this.options)
    }
}
</script>
